using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Services.Obilet;

/// <summary>Keeps one obilet session per visitor.</summary>
public interface IObiletSessionService
{
    /// <summary>Returns the current visitor's obilet session, creating it on the first request.</summary>
    Task<DeviceSession> GetSessionAsync(CancellationToken cancellationToken = default);
}
