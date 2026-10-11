namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The session obilet gives to a visitor; it is sent back with every later request.</summary>
public class DeviceSession
{
    public string SessionId { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
}
