using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Models;

/// <summary>The data shown on the journey search form.</summary>
public class JourneySearchViewModel
{
    public List<BusLocation> Locations { get; set; } = new();
    public int OriginId { get; set; }
    public int DestinationId { get; set; }
    public DateOnly DepartureDate { get; set; }

    // The dates behind the "Bugün" and "Yarın" buttons.
    public DateOnly Today { get; set; }
    public DateOnly Tomorrow { get; set; }
}
