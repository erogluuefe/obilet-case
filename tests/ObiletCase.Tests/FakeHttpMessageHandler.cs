using System.Net;
using System.Text;

namespace ObiletCase.Tests;

// Stands in for the real obilet API: records the request and returns a canned JSON response.
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseJson;

    public FakeHttpMessageHandler(string responseJson)
    {
        _responseJson = responseJson;
    }

    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastRequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        // The body has to be read here; HttpClient disposes it once the request completes.
        LastRequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StringContent(_responseJson, Encoding.UTF8, "application/json");
        return response;
    }
}
