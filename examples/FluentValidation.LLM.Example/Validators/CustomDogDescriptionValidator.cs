using FluentValidation;
using FluentValidation.LLM.Example.Models;
using LLMValidation;
using LLMValidation.FluentValidation;

namespace FluentValidation.LLM.Example.Validators;

/// <summary>
/// Advanced validator demonstrating custom LLM validation with full control.
/// Uses a single comprehensive validation prompt with multiple criteria.
/// </summary>
public class CustomDogDescriptionValidator : AbstractValidator<DogDescription>
{
    public CustomDogDescriptionValidator(ILLMValidator llmValidator)
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        // Custom validation with full control over prompt and options
        RuleFor(x => x.Description)
            .MustPassLLMValidation(llmValidator, options =>
            {
                options.ClientModelName = LLMValidationModelDefaultOption.DefaultClientName;
                options.ValidationPrompt = @"
Validate that this text meets ALL of the following criteria:
1. Has correct grammar and spelling
2. Is about dogs (mentions dogs, dog breeds, or dog behavior)
3. Includes at least one dog breed name
4. Has a professional or friendly tone
";
                options.ErrorMessage = "Description must be well-written, about dogs, mention a breed, and have appropriate tone.";
                options.Temperature = 0.2f;
                options.MaxTokens = 500;
            });
    }
}
