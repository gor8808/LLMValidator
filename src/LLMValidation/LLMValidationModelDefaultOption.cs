namespace LLMValidation;

public class LLMValidationModelDefaultOption
{
    public const string DefaultClientName = nameof(LLMValidationModelDefaultOption);

    /// <summary>
    /// Client model name
    /// </summary>
    public string ClientModelName { get; set; }

    /// <summary>
    /// Optional system prompt to guide the LLM's behavior.
    /// </summary>
    public string SystemPrompt { get; set; } = "You are a helpful assistant that validates responses based on the provided validation prompt. You should answer only to the question that was asked to be able to act fast. This is going to be used in high load scenarios so the answer needs to be quick. If enough evidence is collected to return the data do not analyze more. Keep the reason short. is_valid field should always indicate if the provided text was matching with the condition and should always be there. The reason field is only needed when prompt is not valid";

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
    /// Timeout for the LLM request in milliseconds.
    /// </summary>
    public TimeSpan TimeoutMs { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Additional metadata to pass to the LLM.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];
}
