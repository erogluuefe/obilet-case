namespace ObiletCase.Web.Services.Obilet.Models;

/// <summary>The envelope obilet expects around every request made after GetSession.</summary>
public class ObiletRequest<T>
{
    public T? Data { get; init; }
    public required DeviceSession DeviceSession { get; init; }
    public required string Date { get; init; }
    public required string Language { get; init; }
}
