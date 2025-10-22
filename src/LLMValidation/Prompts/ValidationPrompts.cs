namespace LLMValidation.Prompts;

/// <summary>
/// Contains pre-defined validation prompts optimized for accuracy and performance.
/// </summary>
public static class ValidationPrompts
{
    /// <summary>
    /// Grammar and spelling validation prompt - token optimized.
    /// </summary>
    public const string GrammarAndSpelling = "Validate text for correct grammar, spelling, and punctuation.";

    /// <summary>
    /// Content appropriateness validation prompt - token optimized.
    /// </summary>
    public const string ContentAppropriateness = "Validate text is appropriate and contains no offensive, harmful, or inappropriate content.";

    /// <summary>
    /// Topic validation prompt template - token optimized.
    /// Example: string.Format(TopicValidation, "artificial intelligence")
    /// </summary>
    public const string TopicValidation = "Validate text is primarily about: {0}";

    /// <summary>
    /// Content requirement validation prompt - token optimized.
    /// Example: string.Format(RequiredContent, "pricing information and contact details")
    /// </summary>
    public const string RequiredContent = "Validate text contains: {0}";

    /// <summary>
    /// Tone validation prompt - token optimized.
    /// Example: string.Format(ToneValidation, "professional and courteous")
    /// </summary>
    public const string ToneValidation = "Validate text has a {0} tone.";
}