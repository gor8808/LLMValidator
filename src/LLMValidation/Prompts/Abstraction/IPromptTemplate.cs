namespace LLMValidation.Prompts.Abstraction;

/// <summary>
/// Defines a contract for prompt templates that provide three quality variants.
/// Templates implementing this interface provide parameterless prompts with different
/// levels of detail and accuracy optimization.
/// </summary>
/// <remarks>
/// <para>Use <see cref="PromptTemplateExtensions.GetVariant{T}(PromptVariant)"/> to select variants.</para>
/// </remarks>
public interface IPromptTemplate
{
    /// <summary>
    /// Gets the speed-optimized variant of the prompt.
    /// Designed for high-throughput scenarios where processing speed is critical.
    /// </summary>
    public static abstract string Fast { get; }

    /// <summary>
    /// Gets the balanced variant of the prompt.
    /// Provides good accuracy while maintaining reasonable performance.
    /// This is typically the recommended default for most scenarios.
    /// </summary>
    public static abstract string Balanced { get; }

    /// <summary>
    /// Gets the accuracy-optimized variant of the prompt.
    /// Designed for scenarios where maximum precision is required.
    /// </summary>
    public static abstract string Accurate { get; }
}

/// <summary>
/// Defines a contract for parameterized prompt templates that provide three quality variants.
/// Templates implementing this interface provide prompts that accept parameters for dynamic content.
/// </summary>
/// <typeparam name="TArgs">The type of arguments required by the prompt template.</typeparam>
/// <remarks>
/// <para>Use <see cref="PromptTemplateExtensions.GetVariant{T, TArgs}(PromptVariant, TArgs)"/> to select variants.</para>
/// </remarks>
public interface IPromptTemplate<in TArgs>
{
    /// <summary>
    /// Gets the speed-optimized variant of the parameterized prompt.
    /// Designed for high-throughput scenarios with minimal parameter processing.
    /// </summary>
    /// <param name="args">The arguments to incorporate into the prompt.</param>
    /// <returns>A concise prompt optimized for minimal processing time.</returns>
    public static abstract string Fast(TArgs args);

    /// <summary>
    /// Gets the balanced variant of the parameterized prompt.
    /// Provides good accuracy while maintaining reasonable performance with parameter integration.
    /// This is typically the recommended default for most scenarios.
    /// </summary>
    /// <param name="args">The arguments to incorporate into the prompt.</param>
    /// <returns>A well-structured prompt balancing parameter detail and efficiency.</returns>
    public static abstract string Balanced(TArgs args);

    /// <summary>
    /// Gets the accuracy-optimized variant of the parameterized prompt.
    /// Designed for scenarios where maximum precision is required with comprehensive parameter analysis.
    /// </summary>
    /// <param name="args">The arguments to incorporate into the prompt.</param>
    /// <returns>A comprehensive prompt with detailed parameter-aware instructions.</returns>
    public static abstract string Accurate(TArgs args);
}
