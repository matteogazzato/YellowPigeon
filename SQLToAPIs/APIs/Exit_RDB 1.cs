using E80.CAD.API;
using E80.CAD.API.Entities;
using E80.CAD.API.Entities.Geometry;
using E80.CAD.API.Executable;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Executables;

public class Exit_RDB : IVoidExecutable<IList<ISegment>, ISegmentTemplate>
{
    public string Description => "Add Exit Route Dependant Block";

    public void Execute(
        [Description("List of segments")] IList<ISegment> segments,
        [Description("Template of curve Segment")] ISegmentTemplate FW_curve_template
        )
    {

        Dictionary<ISegment, IList<ISegment>> BlockingObject = new Dictionary<ISegment, IList<ISegment>>();



        int NumberAllloc = 1;

        foreach (var segment in segments)

        {
            ISegmentTemplateProperties template = segment.BaseTemplate;

            if (template.ForwardEnabled == true)
            {
                IList<ISegment> outgoingSegments = SearchSegmentsOutgoings(segment.EndNode);


                foreach (var outgoingSegment in outgoingSegments)
                {
                    
                    if (outgoingSegment.BaseTemplate.Name == FW_curve_template.Name)
                    {   
                        
                        Api.TrafficRules.TryAddManualBlock(blockedEntity: segment,blockingEntity: outgoingSegment,numAlloc: NumberAllloc, description: "Exit route from station");
                        Api.Console.Print($"RDB added on segment ID: {segment.Id} to segment ID: {outgoingSegment.Id}");
                    }
                       
                }

            }
            
        }

    }   

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

  
