namespace ObiletCase.Web.Services.Obilet;

/// <summary>Thrown when obilet answers without a usable result: a status other than "Success" or no data.</summary>
public class ObiletApiException : Exception
{
    public ObiletApiException(string status, string? message, string? userMessage)
        : base(message ?? "obilet API returned status " + status)
    {
        Status = status;
        UserMessage = userMessage;
    }

    public string Status { get; }
    public string? UserMessage { get; }
}
