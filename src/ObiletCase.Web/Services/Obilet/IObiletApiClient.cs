using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Services.Obilet;

/// <summary>Sends requests to the obilet.com business API.</summary>
public interface IObiletApiClient
{
    /// <summary>Asks obilet for a new session for the given visitor.</summary>
    Task<DeviceSession> GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default);
}
