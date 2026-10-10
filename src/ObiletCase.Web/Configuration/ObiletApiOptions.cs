using System.ComponentModel.DataAnnotations;

namespace ObiletCase.Web.Configuration;

/// <summary>obilet.com API settings ("ObiletApi" section).</summary>
public class ObiletApiOptions
{
    public const string SectionName = "ObiletApi";

    [Required, Url]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Token sent in the "Authorization: Basic" header.</summary>
    [Required]
    public string ClientToken { get; set; } = string.Empty;

    [Required]
    public string Language { get; set; } = "tr-TR";
}
