namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The network address of the visitor, sent in a GetSession request.</summary>
public class SessionConnection
{
    public required string IpAddress { get; init; }
    public required string Port { get; init; }
}
