using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Models;

/// <summary>The data shown on the journey page: the search it belongs to and the journeys found for it.</summary>
public class JourneyListViewModel
{
    public int OriginId { get; set; }
    public string? OriginName { get; set; }
    public int DestinationId { get; set; }
    public string? DestinationName { get; set; }
    public DateOnly DepartureDate { get; set; }
    public List<BusJourney> Journeys { get; set; } = new();
}
