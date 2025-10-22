namespace LLMValidation;

/// <summary>
/// Interface for LLM-based string validation.
/// </summary>
public interface ILLMValidator
{
    /// <summary>
    /// Validates a string value using an LLM.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="options">The validation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The validation result.</returns>
    Task<LLMValidationResult> ValidateAsync(
        string value,
        LLMValidationOptions options,
        CancellationToken cancellationToken = default);
}
