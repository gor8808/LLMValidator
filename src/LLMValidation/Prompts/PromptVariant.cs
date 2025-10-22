namespace LLMValidation.Prompts;

/// <summary>
/// Defines different optimization strategies for prompts.
/// </summary>
public enum PromptVariant
{
    /// <summary>
    /// Optimized for speed and minimal token usage. May sacrifice some accuracy.
    /// Best for: High-volume, cost-sensitive applications.
    /// </summary>
    Fast = 1,

    /// <summary>
    /// Balanced approach between speed, accuracy, and token usage.
    /// Best for: Most general use cases.
    /// </summary>
    Balanced = 2,

    /// <summary>
    /// Optimized for maximum accuracy with detailed instructions.
    /// Best for: Critical validations where accuracy is paramount.
    /// </summary>
    Accurate = 3
}