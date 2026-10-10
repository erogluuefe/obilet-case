namespace ObiletCase.Web.Services.Obilet.Models;

public class GetSessionRequest
{
    public int Type { get; init; } = 1;
    public required SessionConnection Connection { get; init; }
    public required SessionBrowser Browser { get; init; }
}
