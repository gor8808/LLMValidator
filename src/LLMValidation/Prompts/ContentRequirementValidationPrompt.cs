using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Validates that text contains or adequately discusses specific required content.
/// Checks for presence, depth, and relevance of required information or concepts.
/// </summary>
/// <remarks>
/// <para><strong>Fast:</strong> Simple content presence check</para>
/// <para><strong>Balanced:</strong> Standard content validation with reasonable coverage assessment</para>
/// <para><strong>Accurate:</strong> Comprehensive content analysis including sufficiency and context</para>
/// <para><strong>Use cases:</strong> Requirement verification, completeness checking, compliance validation</para>
/// <para><strong>Parameter:</strong> Description of the content that must be present</para>
/// </remarks>
public class ContentRequirementValidationPrompt : IPromptTemplate<string>
{
    /// <summary>
    /// Performs basic content presence validation.
    /// Quick check for required content without depth analysis.
    /// </summary>
    /// <param name="requiredContent">The content that must be present</param>
    /// <returns>Simple content requirement prompt</returns>
    public static string Fast(string requiredContent) => $"Must contain {requiredContent}.";

    /// <summary>
    /// Standard content requirement validation with clear criteria.
    /// Checks for mentions, references, and related information.
    /// </summary>
    /// <param name="requiredContent">The content that must be present</param>
    /// <returns>Balanced content validation prompt with structured criteria</returns>
    public static string Balanced(string requiredContent) => $"""
                                                              Check if the text contains or discusses: {requiredContent}

                                                              The text should include:
                                                              - Direct mentions or references
                                                              - Related concepts or terminology
                                                              - Relevant information about the required content
                                                              """;

    /// <summary>
    /// Comprehensive content requirement analysis with detailed evaluation.
    /// Includes sufficiency assessment and contextual relevance evaluation.
    /// </summary>
    /// <param name="requiredContent">The content that must be present</param>
    /// <returns>Detailed content validation prompt with comprehensive criteria</returns>
    public static string Accurate(string requiredContent) => $"""
                                                              Evaluate whether the text adequately contains or addresses: {requiredContent}

                                                              Validation criteria:
                                                              - Direct presence: Look for explicit mentions or references
                                                              - Conceptual inclusion: Related ideas, terminology, or concepts
                                                              - Contextual relevance: Content should be meaningfully related
                                                              - Sufficiency: More than just passing mentions

                                                              Accept:
                                                              - Direct references to {requiredContent}
                                                              - Detailed discussions involving {requiredContent}
                                                              - Technical or specific terminology related to {requiredContent}
                                                              - Examples or cases involving {requiredContent}

                                                              Reject:
                                                              - Vague or tangential mentions
                                                              - Unrelated content
                                                              - Insufficient coverage of the required content
                                                              """;
}