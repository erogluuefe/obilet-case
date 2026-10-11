namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The short name and version of the visitor's browser, sent in a GetSession request.</summary>
public class SessionBrowser
{
    public required string Name { get; init; }
    public required string Version { get; init; }
}
