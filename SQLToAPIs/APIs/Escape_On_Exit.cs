using E80.CAD.API;
using E80.CAD.API.Entities;
using E80.CAD.API.Entities.Geometry;
using E80.CAD.API.Executable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Executables;

/// <summary>
/// Adds a Block Escape (Type B) on warehouse exit segments.
///
/// SQL equivalent: escape_on_Exit.sql
///
/// Logic overview:
///   For each exit segment (12_Fork_FW_Exit_Warehouse), find the immediately
///   following segment that matches the curve template (12_Fork_FW_Curve_v500).
///   When found, register a Block Escape so that if the exit segment is blocked,
///   the AGV is rerouted toward the end of the curve segment.
///
/// JOIN pattern (mirrors SQL):
///   S1.PEndID = S2.PStartID  →  segment.EndNode = outgoingSegment.StartNode
///   This is a sequential path (not a fork): S1 leads directly into S2.
/// </summary>
public class Escape_on_Exit : IVoidExecutable<IList<ISegment>, ISegmentTemplate, string>
{
    public string Description => "Add Block Escape on Exit from Warehouse";

    /// <summary>
    /// Executes the escape configuration logic.
    /// </summary>
    /// <param name="segments">
    ///     List of warehouse exit segments (expected template: 12_Fork_FW_Exit_Warehouse).
    ///     Filtering to the correct template is the responsibility of the caller.
    /// </param>
    /// <param name="outgoingtemplate">
    ///     Template of the curve segment immediately following the exit
    ///     (expected: 12_Fork_FW_Curve_v500).
    /// </param>
    /// <param name="BlockDelay">
    ///     Delay (ms) passed to TryAddBlockEscape as mainDelay AND to TryCreateAgvChainDelay.
    ///     Both delays are driven by this single parameter to keep chain timing consistent.
    ///     Passed as string to align with the existing executable parameter convention.
    /// </param>
    public void Execute(
        [Description("List of segments")] IList<ISegment> segments,
        [Description("Template of curve Segment")] ISegmentTemplate outgoingtemplate,
        [Description("MainBlockDelay: ")] string BlockDelay
        )
    {
        // Fail-fast: validate and parse BlockDelay once before any segment is processed.
        // Parsing inside the loop would cause a partial application if the value is invalid
        // (some segments configured, others skipped due to the thrown exception).
        if (!int.TryParse(BlockDelay, out int Delay_int))
        {
            Api.Console.Print($"[Escape_on_Exit] ERROR: BlockDelay '{BlockDelay}' is not a valid integer. Execution aborted.");
            return;
        }

        // Guard against null or empty input — distinguish bad input from a no-match scenario.
        if (segments == null || segments.Count == 0)
        {
            Api.Console.Print("[Escape_on_Exit] WARNING: No segments provided. Nothing to process.");
            return;
        }

        foreach (var segment in segments)

        {
            // Find all segments that start where this segment ends (S1.PEndID = S2.PStartID)
            IList<ISegment> outgoingSegments = SearchSegmentsOutgoings(segment.EndNode);

            // Track how many curve matches are found for this segment to detect unexpected fork topologies.
            int matchCount = 0;

            foreach (var outgoingSegment in outgoingSegments)
            {
                // Replicate SQL guard: S1.ID <> S2.ID — exclude the segment itself
                // in case of a degenerate zero-length segment where StartNode == EndNode.
                if (outgoingSegment.Id == segment.Id)
                    continue;

                // Match only the curve segment following the exit (S2.TemplateName = '12_Fork_FW_Curve_v500')
                if (outgoingSegment.BaseTemplate.Name == outgoingtemplate.Name)
                {
                    matchCount++;

                    // Warn if more than one curve segment is found at this junction.
                    // The SQL would also produce multiple rows — this makes the behavior explicit.
                    if (matchCount > 1)
                        Api.Console.Print($"[Escape_on_Exit] WARNING: Segment ID {segment.Id} has multiple outgoing curve segments matching template '{outgoingtemplate.Name}'. Multiple escapes will be registered.");

                    // Create the AGV chain delay.
                    // agvCount=1 mirrors SQL ChainBlockedAgvs=1.
                    // delay uses Delay_int (from BlockDelay parameter) to keep chain timing
                    // consistent with the mainDelay passed to TryAddBlockEscape below.
                    var delayCreated = Api.TrafficRules.TryCreateAgvChainDelay(out var delay, 1, Delay_int);

                    // if the delay was not created, we skip adding the escape for this segment
                    if (!delayCreated)
                    {
                        Api.Console.Print($"[Escape_on_Exit] WARNING: Failed to create AGV chain delay for segment ID {segment.Id}. Skipping.");
                        continue;
                    }

                    // Wrap the delay into an AGV chain (required by TryAddBlockEscape)
                    var chainCreated = Api.TrafficRules.TryCreateAgvChain(out var agvChain, [delay]);

                    // if the chain was not created, we skip adding the escape for this segment
                    if (!chainCreated)
                    {
                        Api.Console.Print($"[Escape_on_Exit] WARNING: Failed to create AGV chain for segment ID {segment.Id}. Skipping.");
                        continue;
                    }

                    // Register the Block Escape (Type B):
                    //   - Triggered segment:  segment                  (S1 - the warehouse exit)
                    //   - Escape target node: outgoingSegment.EndNode  (S2.PEndID - end of the curve)
                    //   - priority=1, mainDelay from parameter, chain carries the blocked AGVs
                    //   - Description matches the SQL label exactly: 'B-esc on exit'
                    var escapeCreated = Api.TrafficRules.TryAddBlockEscape(segment, outgoingSegment.EndNode, priority: 1, automatic: null, mainDelay: Delay_int, blockedAgvChain: agvChain, description: $"B-esc on exit");

                    // Check the result of TryAddBlockEscape — a false return means the escape
                    // was not registered (e.g. duplicate). Do not log success unless confirmed.
                    if (escapeCreated)
                        Api.Console.Print($"Block Escape added to segment ID: {segment.Id} to segment ID: {outgoingSegment.Id}");
                    else
                        Api.Console.Print($"[Escape_on_Exit] WARNING: TryAddBlockEscape returned false for segment ID {segment.Id} → {outgoingSegment.Id}. Escape may already exist or was rejected.");
                }

            }

            // Warn when no matching outgoing segment was found — helps detect wrong template
            // passed as input or misconfigured layout, rather than silently doing nothing.
            if (matchCount == 0)
                Api.Console.Print($"[Escape_on_Exit] WARNING: Segment ID {segment.Id} (template: '{segment.BaseTemplate.Name}') had no outgoing segment matching template '{outgoingtemplate.Name}'. No escape registered.");

        }

    }

    /// <summary>
    /// Returns all segments in the repository whose start node matches the given endpoint.
    /// This replicates the SQL join: S1.PEndID = S2.PStartID.
    ///
    /// Note: iterates all repository segments (O(n) per call). For large layouts
    /// with many input segments, this may be a performance bottleneck.
    /// </summary>
    /// <param name="EndPoint">The end node of the upstream segment (S1.PEndID).</param>
    /// <returns>All segments starting at <paramref name="EndPoint"/>.</returns>
    private static IList<ISegment> SearchSegmentsOutgoings(INode EndPoint)
    {

        IList<ISegment> segments = new List<ISegment>();

        // Iterate through all nodessegments in the repository
        foreach (var segment in Api.Repository.Segments)
        {

            if (segment.StartNode.Id == EndPoint.Id)
            {
                segments.Add(segment);
                //Api.Console.Print(segment.Id);
            }

        }

        return segments;
    }
}
