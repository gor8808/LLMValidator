using LLMValidation.Prompts;

namespace LLMValidation;

/// <summary>
/// Default configuration options for LLM validation by model.
/// </summary>
public class LLMValidationModelDefaultOption
{
    /// <summary>
    /// Default client name constant.
    /// </summary>
    public const string DefaultClientName = nameof(LLMValidationModelDefaultOption);

    /// <summary>
    /// The model name.
    /// </summary>
    public string ClientModelName { get; set; }

    /// <summary>
    /// Optional system prompt to guide the LLM's behavior.
    /// </summary>
    public string? SystemPrompt { get; set; } = SystemValidationPrompts.Balanced;

    /// <summary>
    /// Maximum number of tokens for the LLM response.
    /// </summary>
    public int MaxTokens { get; set; } = 150;

    /// <summary>
    /// Temperature for LLM response generation (0.0 to 2.0).
    /// Lower values make output more deterministic.
    /// </summary>
    public float Temperature { get; set; } = 0.1f;

    /// <summary>
    /// Minimum confidence score required for validation to be considered reliable (0.0 to 1.0).
    /// If the LLM's confidence is below this threshold, validation will fail regardless of the result.
    /// </summary>
    public float? MinConfidence { get; set; }

    /// <summary>
    /// Timeout for the LLM request.
    /// </summary>
    public TimeSpan TimeoutMs { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Additional metadata to pass to the LLM.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];
}
