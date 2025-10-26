using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Validates text for grammar and spelling correctness.
/// Checks spelling, grammar rules, punctuation, and sentence structure.
/// </summary>
/// <remarks>
/// <para><strong>Fast:</strong> Quick grammar/spelling check for basic errors</para>
/// <para><strong>Balanced:</strong> Standard grammar validation covering common issues</para>
/// <para><strong>Accurate:</strong> Comprehensive language analysis including style and usage</para>
/// <para><strong>Use cases:</strong> Content moderation, document review, educational feedback</para>
/// </remarks>
public class GrammarValidationPrompts : IPromptTemplate
{
    /// <summary>
    /// Performs quick grammar and spelling validation.
    /// Optimized for speed with basic error detection.
    /// </summary>
    public static string Fast => "Check grammar and spelling quickly.";

    /// <summary>
    /// Standard grammar and spelling validation with common error categories.
    /// Balances thoroughness with reasonable processing time.
    /// </summary>
    public static string Balanced => """
                                     Check if the text has correct grammar and spelling.
                                     Look for:
                                     - Spelling errors
                                     - Basic grammar mistakes
                                     - Sentence structure issues
                                     """;

    /// <summary>
    /// Comprehensive grammar and spelling analysis.
    /// Includes advanced checks for style, usage, and contextual appropriateness.
    /// </summary>
    public static string Accurate => """
                                     Thoroughly evaluate the text for grammar and spelling accuracy.

                                     Check for:
                                     - Spelling errors (including homophones)
                                     - Grammar mistakes (subject-verb agreement, tense consistency)
                                     - Punctuation errors
                                     - Sentence structure and clarity
                                     - Word usage and appropriateness

                                     Consider context and intended meaning. Minor stylistic preferences are acceptable.
                                     """;
}