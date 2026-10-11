using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ObiletCase.Web.Configuration;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Services.Obilet;

/// <summary>Calls the obilet.com business API over HTTP.</summary>
public class ObiletApiClient : IObiletApiClient
{
    // obilet uses kebab-case field names, e.g. "session-id".
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
    };

    private readonly HttpClient _httpClient;
    private readonly ObiletApiOptions _options;

    public ObiletApiClient(HttpClient httpClient, IOptions<ObiletApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<DeviceSession> GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<DeviceSession>("client/getsession", request, cancellationToken);
    }

    public async Task<List<BusLocation>> GetBusLocationsAsync(DeviceSession session, string? searchText, CancellationToken cancellationToken = default)
    {
        var request = new ObiletRequest<string>
        {
            Data = searchText,
            DeviceSession = session,
            Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
            Language = _options.Language
        };

        return await PostAsync<List<BusLocation>>("location/getbuslocations", request, cancellationToken);
    }

    // Every obilet call works the same way: post the request, then read the "data" part of the answer.
    private async Task<TData> PostAsync<TData>(string path, object request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(path, request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ObiletResponse<TData>>(JsonOptions, cancellationToken);

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
