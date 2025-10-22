namespace LLMValidation;

/// <summary>
/// Configuration options for LLM-based validation.
/// </summary>
public class LLMValidationOptions
{
    /// <summary>
    /// The validation prompt that describes what should be validated.
    /// </summary>
    public required string ValidationPrompt { get; set; }

    /// <summary>
    /// The Model Name should match with client name
    /// </summary>
    public required string ClientModelName { get; set; }

    /// <summary>
    /// Optional system prompt to guide the LLM's behavior.
    /// </summary>
    public string? SystemPrompt { get; set; }

    /// <summary>
    /// The custom error message to return when validation fails.
    /// If not provided, the LLM's response will be used.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Maximum number of tokens for the LLM response.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Temperature for LLM response generation (0.0 to 2.0).
    /// Lower values make output more deterministic.
    /// </summary>
    public float? Temperature { get; set; }


    /// <summary>
    /// Timeout for the LLM request in milliseconds.
    /// </summary>
    public TimeSpan TimeoutMs { get; set; }

    /// <summary>
    /// Additional metadata to pass to the LLM.
    /// </summary>
    public Dictionary<string, object?> Metadata { get; set; } = [];

    internal LLMValidationOptions WithDefaults(LLMValidationModelDefaultOption defaultOptions)
    {
        // Create merged metadata
        var mergedMetadata = new Dictionary<string, object?>(Metadata);
        foreach (var defaultMetadata in defaultOptions.Metadata.Where(dm => !mergedMetadata.ContainsKey(dm.Key)))
        {
            mergedMetadata[defaultMetadata.Key] = defaultMetadata.Value;
        }

        return new LLMValidationOptions
        {
            ValidationPrompt = ValidationPrompt,
            ClientModelName = ClientModelName,
            SystemPrompt = SystemPrompt ?? defaultOptions.SystemPrompt,
            ErrorMessage = ErrorMessage,
            MaxTokens = MaxTokens ?? defaultOptions.MaxTokens,
            Temperature = Temperature ?? defaultOptions.Temperature,
            TimeoutMs = TimeoutMs == TimeSpan.Zero ? defaultOptions.TimeoutMs : TimeoutMs,
            Metadata = mergedMetadata
        };
    }
}
