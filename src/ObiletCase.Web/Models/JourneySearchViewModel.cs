using System.ComponentModel.DataAnnotations;
using ObiletCase.Web.Services.Obilet.Models;

namespace ObiletCase.Web.Models;

/// <summary>The data shown on the journey search form, together with its validation rules.</summary>
public class JourneySearchViewModel : IValidatableObject
{
    public List<BusLocation> Locations { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "Kalkış noktası seçiniz.")]
    public int OriginId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Varış noktası seçiniz.")]
    public int DestinationId { get; set; }

    public DateOnly DepartureDate { get; set; }

    // The dates behind the "Bugün" and "Yarın" buttons.
    public DateOnly Today { get; set; }
    public DateOnly Tomorrow { get; set; }

    /// <summary>Rules that compare more than one field; MVC runs them after the attributes above pass.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (OriginId == DestinationId)
        {
            errors.Add(new ValidationResult(
                "Kalkış ve varış noktası aynı olamaz.",
                new[] { nameof(DestinationId) }));
        }

        if (DepartureDate < DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add(new ValidationResult(
                "Yolculuk tarihi bugünden önce olamaz.",
                new[] { nameof(DepartureDate) }));
        }

        return errors;
    }
}
