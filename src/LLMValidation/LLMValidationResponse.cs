using System.Text.Json.Serialization;

namespace LLMValidation;

/// <summary>
/// Structured response format for LLM validation.
/// </summary>
public class LLMValidationResponse
{
    /// <summary>
    /// Whether the validation passed.
    /// </summary>
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    /// <summary>
    /// The reason for validation failure or additional details.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
