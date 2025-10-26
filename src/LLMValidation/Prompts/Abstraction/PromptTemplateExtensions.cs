namespace LLMValidation.Prompts.Abstraction;

/// <summary>
/// Provides utility methods for selecting prompt variants from template classes.
/// This class enables consistent and type-safe prompt variant selection across the validation system.
/// </summary>
/// <remarks>
/// <para>The PromptTemplatePicker works with classes implementing <see cref="IPromptTemplate"/> or <see cref="IPromptTemplate{TArgs}"/>.</para>
/// <para>This approach provides several benefits:</para>
/// </remarks>
public static class PromptTemplateExtensions
{
    /// <summary>
    /// Selects a prompt variant from a non-parameterized template class.
    /// </summary>
    /// <typeparam name="T">The prompt template class implementing <see cref="IPromptTemplate"/>.</typeparam>
    /// <param name="variant">The quality variant to retrieve.</param>
    /// <returns>The prompt string for the specified variant.</returns>
    /// <remarks>
    /// <para>This method provides compile-time type safety and runtime performance for prompt selection.</para>
    /// </remarks>
    public static string GetVariant<T>(PromptVariant variant) where T : IPromptTemplate
    {
        return variant switch
        {
            PromptVariant.Fast => T.Fast,
            PromptVariant.Balanced => T.Balanced,
            PromptVariant.Accurate => T.Accurate,
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Invalid prompt variant specified.")
        };
    }

    /// <summary>
    /// Selects a prompt variant from a parameterized template class with the specified arguments.
    /// </summary>
    /// <typeparam name="T">The prompt template class implementing <see cref="IPromptTemplate{TArgs}"/>.</typeparam>
    /// <typeparam name="TArgs">The type of arguments required by the template.</typeparam>
    /// <param name="variant">The quality variant to retrieve.</param>
    /// <param name="args">The arguments to pass to the template method.</param>
    /// <returns>The prompt string for the specified variant with arguments applied.</returns>
    /// <remarks>
    /// <para>This method enables type-safe parameterized prompt generation with variant selection.</para>
    /// <para>The arguments are passed directly to the template method, allowing for dynamic prompt construction.</para>
    /// <para>Common parameter types include strings for topics, objects for complex validation criteria, and records for structured data.</para>
    /// </remarks>
    public static string GetVariant<T, TArgs>(PromptVariant variant, TArgs args) where T : IPromptTemplate<TArgs>
    {
        return variant switch
        {
            PromptVariant.Fast => T.Fast(args),
            PromptVariant.Balanced => T.Balanced(args),
            PromptVariant.Accurate => T.Accurate(args),
            _ => throw new ArgumentOutOfRangeException(nameof(variant), variant, "Invalid prompt variant specified.")
        };
    }
}