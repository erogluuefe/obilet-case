namespace ObiletCase.Web.Services.Obilet.Models;

public class ObiletResponse<T>
{
    public string Status { get; init; } = string.Empty;
    public T? Data { get; init; }
    public string? Message { get; init; }
    public string? UserMessage { get; init; }
}
