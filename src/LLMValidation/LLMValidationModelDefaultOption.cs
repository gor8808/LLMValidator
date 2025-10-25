using LLMValidation.Prompts;

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
    public IStaticPromptAdapter? SystemPrompt { get; set; } = new StaticPromptAdapter<SystemPrompts.Default>();

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
