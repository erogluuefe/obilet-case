using System.ComponentModel.DataAnnotations;

namespace ObiletCase.Web.Configuration;

/// <summary>Translation settings ("Translations" section).</summary>
public class TranslationOptions
{
    public const string SectionName = "Translations";

    /// <summary>How long a language's translations stay in memory before they are read from the database again.</summary>
    [Range(1, 1440)]
    public int CacheMinutes { get; set; } = 30;
}
