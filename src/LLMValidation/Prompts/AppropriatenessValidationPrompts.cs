using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Validates content for appropriateness, safety, and general audience suitability.
/// Checks for harmful, offensive, or inappropriate material while considering context and intent.
/// </summary>
/// <remarks>
/// <para><strong>Fast:</strong> Basic appropriateness check for obvious violations</para>
/// <para><strong>Balanced:</strong> Standard safety validation covering common inappropriate content</para>
/// <para><strong>Accurate:</strong> Comprehensive content safety analysis with nuanced evaluation</para>
/// <para><strong>Use cases:</strong> Content moderation, compliance checking, platform safety</para>
/// </remarks>
public class AppropriatenessValidationPrompts : IPromptTemplate
{
    /// <summary>
    /// Performs basic content appropriateness validation.
    /// Quick check for obviously inappropriate or unsafe content.
    /// </summary>
    public static string Fast => "Must be appropriate and safe content.";

    /// <summary>
    /// Standard content safety validation with key risk categories.
    /// Covers common inappropriate content types with reasonable detail.
    /// </summary>
    public static string Balanced => """
                                     Check if the content is appropriate and safe for general audiences.

                                     Look for:
                                     - Offensive, harmful, or inappropriate language
                                     - Hate speech or discriminatory content
                                     - Explicit or adult content
                                     - Violence or threatening language
                                     - Misinformation or harmful advice
                                     """;

    /// <summary>
    /// Comprehensive content safety evaluation with detailed criteria.
    /// Includes nuanced assessment considering context, intent, and educational value.
    /// </summary>
    public static string Accurate => """
                                     Thoroughly evaluate the content for appropriateness and safety.

                                     Evaluation criteria:

                                     Reject content containing:
                                     - Hate speech, discrimination, or harassment
                                     - Explicit sexual content or graphic violence
                                     - Personal attacks or bullying
                                     - Harmful misinformation or dangerous advice
                                     - Illegal activities or explicit instructions for harm
                                     - Spam, scams, or fraudulent content

                                     Accept content that:
                                     - Discusses sensitive topics respectfully and constructively
                                     - Contains educational or informational content
                                     - Expresses opinions without attacking individuals or groups
                                     - Uses appropriate language for the context

                                     Consider:
                                     - Context and intent matter
                                     - Educational discussions of sensitive topics are generally acceptable
                                     - News reporting and factual information should be allowed
                                     - Creative content should be evaluated based on overall appropriateness
                                     """;
}