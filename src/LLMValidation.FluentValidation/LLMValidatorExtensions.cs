using FluentValidation;
using LLMValidation.Prompts;
using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.FluentValidation;

/// <summary>
/// Extension methods for integrating LLM validation with FluentValidation.
/// Provides fluent API for common validation scenarios using the new prompt template system.
/// </summary>
/// <remarks>
/// <para>These extensions provide a convenient way to add LLM-based validation rules to FluentValidation validators.</para>
/// <para>All methods support quality variants through <see cref="PromptVariant"/> for different speed/accuracy tradeoffs.</para>
/// <para>Methods with default variants use <see cref="PromptVariant.Balanced"/> for optimal performance/accuracy balance.</para>
/// </remarks>
public static class LLMValidatorExtensions
{
    #region Core Validation Methods

    /// <summary>
    /// Adds an LLM-based validation rule to a string property with a custom prompt.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="validationPrompt">The custom prompt describing what to validate.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.Description)
    ///     .MustPassLLMValidation(validator, "Check if the text describes a technical product clearly");
    /// </code>
    /// </example>
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
    /// Adds an LLM-based validation rule to a string property with comprehensive options.
    /// This is the core validation method that all other extensions use internally.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="options">The complete validation options including prompt, model, and configuration.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.Content)
    ///     .MustPassLLMValidation(validator, new LLMValidationOptions
    ///     {
    ///         ValidationPrompt = PromptTemplatePicker.GetVariant&lt;ValidationPrompts.Grammar&gt;(PromptVariant.Accurate),
    ///         ClientModelName = "gpt-4",
    ///         Temperature = 0.0f,
    ///         ErrorMessage = "Content must have perfect grammar for publication."
    ///     });
    /// </code>
    /// </example>
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

    #endregion

    #region Grammar & Spelling Validation

    /// <summary>
    /// Adds an LLM-based validation rule that checks grammar and spelling with balanced quality.
    /// Uses the default balanced variant for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.Description)
    ///     .MustHaveValidGrammar(validator);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustHaveValidGrammar<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string? clientModelName = null)
    {
        return ruleBuilder.MustHaveValidGrammar(validator, PromptVariant.Balanced, clientModelName);
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks grammar and spelling with specified quality variant.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Fast grammar check for high-volume processing
    /// RuleFor(x => x.Comment)
    ///     .MustHaveValidGrammar(validator, PromptVariant.Fast);
    ///
    /// // Accurate grammar check for published content
    /// RuleFor(x => x.Article)
    ///     .MustHaveValidGrammar(validator, PromptVariant.Accurate, "gpt-4");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustHaveValidGrammar<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(variant),
            ErrorMessage = "The text contains grammar or spelling errors."
        });
    }

    #endregion

    #region Topic Validation

    /// <summary>
    /// Adds an LLM-based validation rule that checks if the content is about a specific topic with balanced quality.
    /// Uses the default balanced variant for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="topic">The topic the content should be about.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.BlogPost)
    ///     .MustBeAbout(validator, "machine learning");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeAbout<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string topic,
        string? clientModelName = null)
    {
        return ruleBuilder.MustBeAbout(validator, topic, PromptVariant.Balanced, clientModelName);
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if the content is about a specific topic with specified quality variant.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="topic">The topic the content should be about.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Fast topic check for content filtering
    /// RuleFor(x => x.UserPost)
    ///     .MustBeAbout(validator, "technology", PromptVariant.Fast);
    ///
    /// // Accurate topic validation for content curation
    /// RuleFor(x => x.Article)
    ///     .MustBeAbout(validator, "artificial intelligence", PromptVariant.Accurate, "gpt-4");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeAbout<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string topic,
        PromptVariant variant,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(variant, topic),
            ErrorMessage = $"The text must be about {topic}."
        });
    }

    #endregion

    #region Content Requirements Validation

    /// <summary>
    /// Adds an LLM-based validation rule that checks if the content contains specific required information with balanced quality.
    /// Uses the default balanced variant for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="requiredContent">Description of the content that must be present.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.ProductDescription)
    ///     .MustContain(validator, "pricing information and technical specifications");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustContainContent<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string requiredContent,
        string? clientModelName = null)
    {
        return ruleBuilder.MustContainContent(validator, requiredContent, PromptVariant.Balanced, clientModelName);
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if the content contains specific required information with specified quality variant.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="requiredContent">Description of the content that must be present.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Fast content check for basic requirements
    /// RuleFor(x => x.JobDescription)
    ///     .MustContain(validator, "salary range", PromptVariant.Fast);
    ///
    /// // Accurate content validation for compliance
    /// RuleFor(x => x.LegalDocument)
    ///     .MustContain(validator, "liability disclaimers and terms of service", PromptVariant.Accurate);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustContainContent<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string requiredContent,
        PromptVariant variant,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<ContentRequirementValidationPrompt, string>(variant, requiredContent),
            ErrorMessage = $"The text must contain {requiredContent}."
        });
    }

    #endregion

    #region Tone Validation

    /// <summary>
    /// Adds an LLM-based validation rule that checks tone and sentiment with balanced quality.
    /// Uses the default balanced variant for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="expectedTone">The expected tone (e.g., "professional", "friendly", "formal", "casual").</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.CustomerEmail)
    ///     .MustHaveTone(validator, "professional");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustHaveTone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string expectedTone,
        string? clientModelName = null)
    {
        return ruleBuilder.MustHaveTone(validator, expectedTone, PromptVariant.Balanced, clientModelName);
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks tone and sentiment with specified quality variant.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="expectedTone">The expected tone (e.g., "professional", "friendly", "formal", "casual").</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Fast tone check for social media posts
    /// RuleFor(x => x.SocialPost)
    ///     .MustHaveTone(validator, "friendly", PromptVariant.Fast);
    ///
    /// // Accurate tone validation for brand communications
    /// RuleFor(x => x.PressRelease)
    ///     .MustHaveTone(validator, "professional", PromptVariant.Accurate, "gpt-4");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustHaveTone<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string expectedTone,
        PromptVariant variant,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(variant, expectedTone),
            ErrorMessage = $"The text must have a {expectedTone} tone."
        });
    }

    #endregion

    #region Content Appropriateness Validation

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content is appropriate and safe with balanced quality.
    /// Uses the default balanced variant for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.UserComment)
    ///     .MustBeAppropriate(validator);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeAppropriate<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string? clientModelName = null)
    {
        return ruleBuilder.MustBeAppropriate(validator, PromptVariant.Balanced, clientModelName);
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content is appropriate and safe with specified quality variant.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Fast appropriateness check for user-generated content
    /// RuleFor(x => x.ForumPost)
    ///     .MustBeAppropriate(validator, PromptVariant.Fast);
    ///
    /// // Accurate appropriateness validation for published content
    /// RuleFor(x => x.PublishedArticle)
    ///     .MustBeAppropriate(validator, PromptVariant.Accurate, "gpt-4");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeAppropriate<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(variant),
            ErrorMessage = "The text contains inappropriate content."
        });
    }

    #endregion

    #region Advanced Use Case Extensions

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents a professional business email.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.EmailContent)
    ///     .MustBeProfessionalEmail(validator, PromptVariant.Accurate);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeProfessionalEmail<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(variant),
            ErrorMessage = "The text must be a professional business email."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents a well-structured business proposal.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.Proposal)
    ///     .MustBeBusinessProposal(validator, PromptVariant.Accurate, "gpt-4");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeBusinessProposal<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.ProposalValidation>(variant),
            ErrorMessage = "The text must be a well-structured business proposal."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents clear code documentation.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.ApiDocumentation)
    ///     .MustBeCodeDocumentation(validator, PromptVariant.Accurate);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeCodeDocumentation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<TechnicalContentValidationTemplates.CodeDocumentation>(variant),
            ErrorMessage = "The text must be clear and helpful code documentation."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents comprehensive API documentation.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.ApiDocumentation)
    ///     .MustBeAPIDocumentation(validator, PromptVariant.Accurate);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeAPIDocumentation<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<TechnicalContentValidationTemplates.APIDocumentation>(variant),
            ErrorMessage = "The text must be comprehensive API documentation."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents structured educational content.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.LessonContent)
    ///     .MustBeEducationalContent(validator, PromptVariant.Accurate);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeEducationalContent<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<EducationalContentValidationTemplates.LessonContent>(variant),
            ErrorMessage = "The text must be structured educational content."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents an engaging product description.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.ProductDescription)
    ///     .MustBeProductDescription(validator, PromptVariant.Accurate, "gpt-4");
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeProductDescription<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<CreativeContentValidationTemplates.ProductDescription>(variant),
            ErrorMessage = "The text must be an engaging product description."
        });
    }

    /// <summary>
    /// Adds an LLM-based validation rule that checks if content represents a well-written blog post.
    /// Uses balanced quality for optimal performance/accuracy tradeoff.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="validator">The LLM validator instance.</param>
    /// <param name="variant">The quality variant to use (Fast/Balanced/Accurate).</param>
    /// <param name="clientModelName">Optional model name. If null, uses the default client.</param>
    /// <returns>The rule builder for chaining.</returns>
    /// <example>
    /// <code>
    /// RuleFor(x => x.BlogPost)
    ///     .MustBeBlogPost(validator, PromptVariant.Accurate);
    /// </code>
    /// </example>
    public static IRuleBuilderOptions<T, string?> MustBeBlogPost<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<CreativeContentValidationTemplates.BlogPost>(variant),
            ErrorMessage = "The text must be a well-written blog post."
        });
    }

    #endregion
}