namespace ObiletCase.Web.Localization;

/// <summary>One text of the site in one language, e.g. "FindTickets" in "en-US" is "Find Tickets".</summary>
public class Translation
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
