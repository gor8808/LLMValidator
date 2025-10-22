using FluentValidation;
using LLMValidation;
using LLMValidation.Prompts;

namespace LLMValidation.FluentValidation;

/// <summary>
/// Extension methods for integrating LLM validation with FluentValidation.
/// </summary>
public static class LLMValidatorExtensions
{
    /// <summary>
    /// Adds an LLM-based validation rule to a string property.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="validationPrompt">The prompt describing what to validate.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustPassLLMValidation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string validationPrompt,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = validationPrompt
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule to a string property with custom options.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="options">The validation options.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustPassLLMValidation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        LLMValidationOptions options)
    {
        return ruleBuilder.MustAsync(async (_, value, context, cancellation) =>
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var result = await validator.ValidateAsync(value, options, cancellation);

            // Only add failure if validation failed
            if (!result.IsValid)
            {
                context.AddFailure(options.ErrorMessage ?? result.Message ?? "Validation failed");
            }

            return result.IsValid;
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if the content is about a specific topic.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="topic">The topic the content should be about.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustBeAbout<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string topic,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = string.Format(ValidationPrompts.TopicValidation, topic),
            ErrorMessage = $"The text must be about {topic}."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks grammar and spelling.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustHaveValidGrammar<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = ValidationPrompts.GrammarAndSpelling,
            ErrorMessage = "The text contains grammar or spelling errors."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if the content contains specific information.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="requiredContent">Description of the required content.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustContain<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string requiredContent,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = string.Format(ValidationPrompts.RequiredContent, requiredContent),
            ErrorMessage = $"The text must contain {requiredContent}."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks tone and sentiment.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="tone">The expected tone (e.g., "professional", "friendly", "neutral").</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustHaveTone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string tone,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = string.Format(ValidationPrompts.ToneValidation, tone),
            ErrorMessage = $"The text must have a {tone} tone."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content is appropriate and safe.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    public static IRuleBuilderOptions<T, string?> MustBeAppropriate<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = ValidationPrompts.ContentAppropriateness,
            ErrorMessage = "The text contains inappropriate content."
        });
    }
}
