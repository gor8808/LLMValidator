using FluentValidation;
using LLMValidation.Example.Api.Models;
using LLMValidation;
using LLMValidation.FluentValidation;

namespace LLMValidation.Example.Api.Validators;

/// <summary>
/// Validator for dog descriptions using multiple LLM validation rules.
/// </summary>
public class DogDescriptionValidator : AbstractValidator<DogDescription>
{
    public DogDescriptionValidator(ILLMValidator llmValidator)
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.Description)
            .MustHaveValidGrammar(llmValidator)
            .WithMessage("Description must have valid grammar.");

        RuleFor(x => x.Description)
            .MustBeAbout(llmValidator, "dogs")
            .WithMessage("Description must be about dogs.");

        RuleFor(x => x.Description)
            .MustContainContent(llmValidator, "dog breed names or dog-related terminology")
            .WithMessage("Description should mention dog breeds or dog-related terms.");
    }
}
