namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The "data" part of a GetJourneys request: which route is asked for and on which day.</summary>
public class JourneyQuery
{
    public required int OriginId { get; init; }
    public required int DestinationId { get; init; }
    public required string DepartureDate { get; init; }
}
