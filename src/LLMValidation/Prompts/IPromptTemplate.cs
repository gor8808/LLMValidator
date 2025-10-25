namespace LLMValidation.Prompts;

/// <summary>
/// Simple prompt template with parameter support and variants.
/// </summary>
public interface IPromptTemplate<TArg> : IPromptTemplate
{
    public TArg Arguments { get; set; }
}

/// <summary>
/// Simple prompt template with parameter support and variants.
/// </summary>
public interface IPromptTemplate
{
    public static abstract string FastPrompt { get; }
    public static abstract string BalancedPrompt { get; }
    public static abstract string AccuratePrompt { get; }
}