using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Validates that text content is relevant to and primarily focused on a specified topic.
/// Evaluates topical relevance, depth of discussion, and contextual appropriateness.
/// </summary>
/// <remarks>
/// <para><strong>Fast:</strong> Simple topic matching for quick filtering</para>
/// <para><strong>Balanced:</strong> Standard topic relevance with reasonable depth assessment</para>
/// <para><strong>Accurate:</strong> Comprehensive topic analysis including context and intent</para>
/// <para><strong>Use cases:</strong> Content curation, search relevance, category validation</para>
/// <para><strong>Parameter:</strong> Topic name or description to validate against</para>
/// </remarks>
public class TopicValidationPrompts : IPromptTemplate<string>
{
    /// <summary>
    /// Performs basic topic matching validation.
    /// Quick check for topic relevance without deep analysis.
    /// </summary>
    /// <param name="topic">The topic the text should be about</param>
    /// <returns>Simple topic validation prompt</returns>
    public static string Fast(string topic) => $"Must be about {topic}.";

    /// <summary>
    /// Standard topic relevance validation with clear criteria.
    /// Checks for focus, relevance, and basic topical alignment.
    /// </summary>
    /// <param name="topic">The topic the text should be about</param>
    /// <returns>Balanced topic validation prompt with structured criteria</returns>
    public static string Balanced(string topic) => $"""
                                                    Check if the text is relevant to and primarily about: {topic}

                                                    The text should:
                                                    - Focus on the specified topic
                                                    - Contain relevant information or discussion
                                                    - Not be off-topic or unrelated
                                                    """;

    /// <summary>
    /// Comprehensive topic analysis with detailed evaluation criteria.
    /// Includes context assessment, intent evaluation, and nuanced topic matching.
    /// </summary>
    /// <param name="topic">The topic the text should be about</param>
    /// <returns>Detailed topic validation prompt with comprehensive criteria</returns>
    public static string Accurate(string topic) => $"""
                                                    Evaluate whether the text is genuinely about: {topic}

                                                    Validation criteria:
                                                    - Primary focus: The main subject should be {topic}
                                                    - Relevance: Content should be directly related to {topic}
                                                    - Context: Consider if mentions are substantial, not just passing references
                                                    - Intent: The text should demonstrate knowledge or discussion of {topic}

                                                    Accept:
                                                    - Text that discusses, explains, or relates to {topic}
                                                    - Content that uses {topic} as a central theme
                                                    - Comparative discussions involving {topic}

                                                    Reject:
                                                    - Brief mentions without substantial discussion
                                                    - Off-topic content with tangential references
                                                    - Completely unrelated content
                                                    """;
}