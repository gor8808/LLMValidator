namespace LLMValidation.Prompts;

/// <summary>
/// Simple prompt template with parameter support and variants.
/// </summary>
public interface IPromptTemplate<in TArg>
{
    /// <summary>
    /// Builds the prompt using the specified variant and parameters.
    /// </summary>
    /// <param name="variant">The prompt variant to use.</param>
    /// <param name="argument">The parameters to substitute. Can be null if no parameters required.</param>
    /// <returns>The final prompt string.</returns>
    static abstract string Build(PromptVariant variant, TArg argument);
}

/// <summary>
/// Simple prompt template with parameter support and variants.
/// </summary>
public interface IPromptTemplate
{
    /// <summary>
    /// Builds the prompt using the specified variant and parameters.
    /// </summary>
    /// <param name="variant">The prompt variant to use.</param>
    /// <returns>The final prompt string.</returns>
    static abstract string Build(PromptVariant variant);
}