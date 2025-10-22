using FluentValidation.LLM.Example.Models;
using FluentValidation.LLM.Example.Validators;

namespace FluentValidation.LLM.Example.Endpoints;

/// <summary>
/// Defines API endpoints for dog description validation.
/// </summary>
public static class ValidationEndpoints
{
    public static void MapValidationEndpoints(this WebApplication app)
    {
        var validationGroup = app.MapGroup("/api/validate")
            .WithTags("Validation")
            .WithOpenApi();

        // Standard validation with multiple rules
        validationGroup.MapPost("/", ValidateDescription)
            .WithName("ValidateDescription")
            .WithSummary("Validates dog description using standard rules")
            .WithDescription("Applies grammar check, topic validation, and content requirements separately.");

        // Custom validation with combined prompt
        validationGroup.MapPost("/custom", ValidateDescriptionCustom)
            .WithName("ValidateDescriptionCustom")
            .WithSummary("Validates dog description using custom combined validation")
            .WithDescription("Uses a single comprehensive prompt to check all criteria at once.");
    }

    private static async Task<IResult> ValidateDescription(
        DogDescription model,
        DogDescriptionValidator validator)
    {
        var result = await validator.ValidateAsync(model);

        if (result.IsValid)
        {
            return Results.Ok(new ValidationResponse
            {
                IsValid = true,
                Message = "Description is valid!"
            });
        }

        return Results.BadRequest(new ValidationResponse
        {
            IsValid = false,
            Errors = result.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            }).ToList()
        });
    }

    private static async Task<IResult> ValidateDescriptionCustom(
        DogDescription model,
        CustomDogDescriptionValidator validator)
    {
        var result = await validator.ValidateAsync(model);

        if (result.IsValid)
        {
            return Results.Ok(new ValidationResponse
            {
                IsValid = true,
                Message = "Description meets all custom criteria!"
            });
        }

        return Results.BadRequest(new ValidationResponse
        {
            IsValid = false,
            Errors = result.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            }).ToList()
        });
    }
}
