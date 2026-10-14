using System.ComponentModel.DataAnnotations;

namespace ObiletCase.Web.Configuration;

/// <summary>Localization settings ("Localization" section).</summary>
public class LocalizationOptions
{
    public const string SectionName = "Localization";

    /// <summary>How long a language's translations stay in memory before they are read from the database again.</summary>
    [Range(1, 1440)]
    public int CacheMinutes { get; set; } = 30;
}
