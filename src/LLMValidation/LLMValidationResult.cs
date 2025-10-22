namespace LLMValidation;

/// <summary>
/// Represents the result of an LLM validation.
/// </summary>
public class LLMValidationResult
{
    /// <summary>
    /// Whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The reason for validation failure or additional details.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The raw response from the LLM.
    /// </summary>
    public string? RawResponse { get; set; }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static LLMValidationResult Success(string? message = null, string? rawResponse = null)
    {
        return new LLMValidationResult
        {
            IsValid = true,
            Message = message,
            RawResponse = rawResponse
        };
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    public static LLMValidationResult Failure(string message, string? rawResponse = null)
    {
        return new LLMValidationResult
        {
            IsValid = false,
            Message = message,
            RawResponse = rawResponse
        };
    }
}
