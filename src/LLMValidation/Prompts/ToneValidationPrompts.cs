using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Validates that text maintains an appropriate tone and communication style.
/// Evaluates language formality, emotional quality, and stylistic consistency.
/// </summary>
/// <remarks>
/// <para><strong>Fast:</strong> Basic tone matching validation</para>
/// <para><strong>Balanced:</strong> Standard tone analysis with key style factors</para>
/// <para><strong>Accurate:</strong> Comprehensive tone evaluation including consistency and context</para>
/// <para><strong>Use cases:</strong> Brand voice compliance, communication standards, audience appropriateness</para>
/// <para><strong>Parameter:</strong> Expected tone (e.g., "professional", "friendly", "formal", "casual")</para>
/// </remarks>
public class ToneValidationPrompts : IPromptTemplate<string>
{
    /// <summary>
    /// Performs basic tone validation.
    /// Quick check for expected tone without detailed analysis.
    /// </summary>
    /// <param name="expectedTone">The expected tone style</param>
    /// <returns>Simple tone validation prompt</returns>
    public static string Fast(string expectedTone) => $"Must have {expectedTone} tone.";

    /// <summary>
    /// Standard tone validation with key evaluation factors.
    /// Assesses word choice, formality, and emotional undertones.
    /// </summary>
    /// <param name="expectedTone">The expected tone style</param>
    /// <returns>Balanced tone validation prompt with clear criteria</returns>
    public static string Balanced(string expectedTone) => $"""
                                                           Check if the text has a {expectedTone} tone.

                                                           Consider:
                                                           - Word choice and language style
                                                           - Formality level
                                                           - Emotional undertone
                                                           - Overall communication style
                                                           """;

    /// <summary>
    /// Comprehensive tone analysis with detailed evaluation criteria.
    /// Includes consistency assessment and context-specific examples.
    /// </summary>
    /// <param name="expectedTone">The expected tone style</param>
    /// <returns>Detailed tone validation prompt with comprehensive analysis</returns>
    public static string Accurate(string expectedTone) => $"""
                                                           Evaluate whether the text maintains a {expectedTone} tone throughout.

                                                           Analysis criteria:
                                                           - Language style: Formal, informal, casual, technical, etc.
                                                           - Word choice: Professional, friendly, authoritative, conversational
                                                           - Emotional quality: Positive, neutral, serious, enthusiastic, etc.
                                                           - Consistency: Tone should be maintained throughout the text
                                                           - Appropriateness: Tone should match the expected style

                                                           Examples of {expectedTone} tone:
                                                           {GetToneExamples(expectedTone)}

                                                           Consider context and purpose. Minor variations are acceptable if the overall tone aligns.
                                                           """;

    /// <summary>
    /// Provides context-specific examples for common tone types.
    /// </summary>
    /// <param name="tone">The tone type to provide examples for</param>
    /// <returns>Formatted examples string for the specified tone</returns>
    private static string GetToneExamples(string tone) => tone.ToLowerInvariant() switch
    {
        "professional" =>
            "- Clear, respectful, business-appropriate language\n- Avoiding slang, overly casual expressions\n- Structured and well-organized presentation",
        "friendly" =>
            "- Warm, welcoming language\n- Conversational but respectful\n- Positive and approachable expressions",
        "formal" =>
            "- Academic or official language\n- Complete sentences and proper grammar\n- Avoiding contractions and casual expressions",
        "casual" =>
            "- Relaxed, conversational language\n- May include contractions and informal expressions\n- Approachable and easy-going style",
        _ =>
            "- Language appropriate for the specified tone\n- Consistent style throughout the text\n- Context-appropriate expressions"
    };
}