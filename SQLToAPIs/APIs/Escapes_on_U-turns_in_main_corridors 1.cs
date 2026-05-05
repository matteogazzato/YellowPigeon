using E80.CAD.API;
using E80.CAD.API.Entities;
using E80.CAD.API.Entities.Geometry;
using E80.CAD.API.Executable;
using E80.CAD.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Executables;

public class Escape_on_UTurn : IVoidExecutable<IList<ISegment>, ISegmentTemplate, string, string>
{
    public string Description => "Add Block and Deadlock escapes on U_Turn to go straight";

    public void Execute(
        [Description("List of segments")] IList<ISegment> segments,
        [Description("Template of segment to escape on it")] ISegmentTemplate outgoingtemplate,
        [Description("MainBlockDelay: ")] string BlockDelay,
        [Description("MainDeadlockDelay: ")] string DeadlockDelay
        )
    {

        Dictionary<ISegment, IList<ISegment>> BlockingObject = new Dictionary<ISegment, IList<ISegment>>();

        

        foreach (var segment in segments)

        {
            IList<ISegment> outgoingSegments = SearchSegmentsOutgoings(segment.StartNode);
            int Delay_int = Int32.Parse(BlockDelay);
            int DeadDelay_int = Int32.Parse(DeadlockDelay);



            foreach (var outgoingSegment in outgoingSegments)
            {
                if (outgoingSegment.BaseTemplate.Name == outgoingtemplate.Name)
                {
                    var delayCreated = Api.TrafficRules.TryCreateAgvChainDelay(out var delay, 1, 100);

                    // if the delay was not created, we skip adding the escape for this segment
                    if (!delayCreated)
                        continue;

                    var chainCreated = Api.TrafficRules.TryCreateAgvChain(out var agvChain, [delay]);

                    // if the chain was not created, we skip adding the escape for this segment
                    if (!chainCreated)
                        continue;
                    Api.TrafficRules.TryAddBlockEscape(segment, outgoingSegment.EndNode, priority: 1, automatic: null, mainDelay: Delay_int, blockedAgvChain: agvChain, description: $"B-esc on U-turns in main corridors");
                    Api.Console.Print($"Block Escape added to segment ID: {outgoingSegment}");


                    Api.TrafficRules.TryAddDeadlockEscape(segment, outgoingSegment.EndNode, priority: 1, automatic: null, mainDelay: DeadDelay_int, description: $"D-esc on U-turns in main corridors");
                    Api.Console.Print($"Deadlock Escape added to segment ID: {outgoingSegment}");
                }

            }
                        

        }



    }


    private static IList<ISegment> SearchSegmentsOutgoings(INode StartPoint)
    {

        IList<ISegment> segments = new List<ISegment>();
        

        // Iterate through all nodessegments in the repository
        foreach (var segment in Api.Repository.Segments)
        {

            if (segment.StartNode.Id == StartPoint.Id)
            {
                segments.Add(segment);
                Api.Console.Print(segment.Id);
            }

        }

        return segments;
    }


  
    
    public interface IAgvChainDelay : IEntity, IEquatable<IEntity>
    {
        //
        // Summary:
        //     Gets the number of AGVs in the chain.
        int AgvCount { get; }

        //
        // Summary:
        //     Gets the delay value associated with the AGV chain.
        int Delay { get; }
    }
   

}
