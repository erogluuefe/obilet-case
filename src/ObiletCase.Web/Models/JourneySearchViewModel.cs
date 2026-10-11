using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Models;

/// <summary>The data shown on the journey search form.</summary>
public class JourneySearchViewModel
{
    public List<BusLocation> Locations { get; set; } = new();
    public int OriginId { get; set; }
    public int DestinationId { get; set; }
    public DateOnly DepartureDate { get; set; }
}
