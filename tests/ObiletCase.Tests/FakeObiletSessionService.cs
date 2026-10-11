using ObiletCase.Web.Services.Obilet;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Tests;

/// <summary>Stands in for the session service: always returns the same visitor session and counts how often it is forgotten.</summary>
public class FakeObiletSessionService : IObiletSessionService
{
    public DeviceSession Session { get; } = new DeviceSession { SessionId = "visitor-session", DeviceId = "visitor-device" };
    public int ForgetSessionCallCount { get; private set; }

    public Task<DeviceSession> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Session);
    }

    public void ForgetSession()
    {
        ForgetSessionCallCount++;
    }
}
