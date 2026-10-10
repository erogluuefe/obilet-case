using System.Text.Json;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Services.Obilet;

public class ObiletApiClient : IObiletApiClient
{
    // obilet uses kebab-case field names, e.g. "session-id".
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
    };

    private readonly HttpClient _httpClient;

    public ObiletApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DeviceSession> GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("client/getsession", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ObiletResponse<DeviceSession>>(JsonOptions, cancellationToken);

        if (result == null)
        {
            throw new ObiletApiException("Unknown", "obilet API returned an empty response.", null);
        }

        // obilet reports failures in the "status" field, not with the HTTP status code.
        if (result.Status != "Success" || result.Data == null)
        {
            throw new ObiletApiException(result.Status, result.Message, result.UserMessage);
        }

        return result.Data;
    }
}
