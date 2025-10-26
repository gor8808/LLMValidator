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
    [JsonPropertyName("v")]
    public bool IsValid { get; set; }

    /// <summary>
    /// The reason for validation failure or additional details.
    /// </summary>
    [JsonPropertyName("r")]
    public string? Reason { get; set; }

    /// <summary>
    /// Confidence score from 0.0 to 1.0, where 1.0 represents maximum confidence.
    /// </summary>
    [JsonPropertyName("c")]
    public float? Confidence { get; set; }
}
