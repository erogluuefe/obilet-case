namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The stops, times and price of a bus journey.</summary>
public class JourneyDetails
{
    public string Origin { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public DateTime Departure { get; init; }
    public DateTime Arrival { get; init; }
    public decimal InternetPrice { get; init; }
    public string Currency { get; init; } = string.Empty;
}
