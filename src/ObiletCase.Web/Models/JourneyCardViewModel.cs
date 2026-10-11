namespace ObiletCase.Web.Models;

/// <summary>One journey card on the journey page, with every decision already made so the view only prints it.</summary>
public class JourneyCardViewModel
{
    public string PartnerName { get; set; } = string.Empty;
    public string OriginStop { get; set; } = string.Empty;
    public string DestinationStop { get; set; } = string.Empty;
    public DateTime Departure { get; set; }
    public DateTime Arrival { get; set; }

    // True when the journey leaves on the day after the searched one.
    public bool DepartsNextDay { get; set; }

    public decimal Price { get; set; }

    // The currency the way it is shown on the card, e.g. "TL".
    public string CurrencyLabel { get; set; } = string.Empty;
}
