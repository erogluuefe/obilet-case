using System.ComponentModel.DataAnnotations;
using ObiletCase.Web.Models;
using ObiletCase.Web.Services;

namespace ObiletCase.Tests;

public class JourneySearchViewModelTests
{
    private static readonly DateOnly Today = TurkeyTime.Today;

    [Fact]
    public void DifferentLocationsAndTomorrow_IsValid()
    {
        var model = CreateModel(originId: 349, destinationId: 356, departureDate: Today.AddDays(1));

        var errors = Validate(model);

        Assert.Empty(errors);
    }

    [Fact]
    public void Today_IsTheEarliestValidDepartureDate()
    {
        var model = CreateModel(originId: 349, destinationId: 356, departureDate: Today);

        var errors = Validate(model);

        Assert.Empty(errors);
    }

    [Fact]
    public void SameOriginAndDestination_IsRejectedOnTheDestinationField()
    {
        var model = CreateModel(originId: 349, destinationId: 349, departureDate: Today);

        var errors = Validate(model);

        var error = Assert.Single(errors);
        Assert.Equal("Kalkış ve varış noktası aynı olamaz.", error.ErrorMessage);
        Assert.Contains(nameof(JourneySearchViewModel.DestinationId), error.MemberNames);
    }

    [Fact]
    public void Yesterday_IsRejectedOnTheDateField()
    {
        var model = CreateModel(originId: 349, destinationId: 356, departureDate: Today.AddDays(-1));

        var errors = Validate(model);

        var error = Assert.Single(errors);
        Assert.Equal("Yolculuk tarihi bugünden önce olamaz.", error.ErrorMessage);
        Assert.Contains(nameof(JourneySearchViewModel.DepartureDate), error.MemberNames);
    }

    [Fact]
    public void MissingOrigin_IsRejected()
    {
        var model = CreateModel(originId: 0, destinationId: 356, departureDate: Today);

        var errors = Validate(model);

        var error = Assert.Single(errors);
        Assert.Equal("Kalkış noktası seçiniz.", error.ErrorMessage);
    }

    private static JourneySearchViewModel CreateModel(int originId, int destinationId, DateOnly departureDate)
    {
        return new JourneySearchViewModel
        {
            OriginId = originId,
            DestinationId = destinationId,
            DepartureDate = departureDate
        };
    }

    // Runs the attributes and the Validate method the same way MVC does when a form is posted.
    private static List<ValidationResult> Validate(JourneySearchViewModel model)
    {
        var errors = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, errors, validateAllProperties: true);

        return errors;
    }
}
