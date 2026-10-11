namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>One bus journey from a GetJourneys response; only the fields the journey page shows are read.</summary>
public class BusJourney
{
    public string PartnerName { get; init; } = string.Empty;
    public string OriginLocation { get; init; } = string.Empty;
    public string DestinationLocation { get; init; } = string.Empty;
    public JourneyDetails Journey { get; init; } = new();
}
