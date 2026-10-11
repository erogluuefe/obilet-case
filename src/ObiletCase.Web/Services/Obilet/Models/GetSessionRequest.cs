namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The body of a GetSession request: who the visitor is, as far as the server can tell.</summary>
public class GetSessionRequest
{
    public int Type { get; init; } = 1;
    public required SessionConnection Connection { get; init; }
    public required SessionBrowser Browser { get; init; }
}
