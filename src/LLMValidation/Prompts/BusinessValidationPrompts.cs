using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Business and professional content validation templates.
/// Specialized prompts for workplace communication, documentation, and professional writing.
/// </summary>
public class BusinessValidationPrompts
{
    /// <summary>
    /// Validates professional business email communication.
    /// Checks structure, tone, clarity, and business appropriateness.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> Email validation, communication standards, professional correspondence</para>
    /// </remarks>
    public class EmailValidation : IPromptTemplate
    {
        /// <summary>
        /// Quick professional email validation.
        /// Basic check for business communication standards.
        /// </summary>
        public static string Fast => "Must be professional business email.";

        /// <summary>
        /// Standard business email validation with key requirements.
        /// Checks essential elements of professional communication.
        /// </summary>
        public static string Balanced => """
                                         Check if this is a professional business email.

                                         Requirements:
                                         - Professional tone and language
                                         - Clear subject or purpose
                                         - Appropriate greeting and closing
                                         - Business-appropriate content
                                         """;

        /// <summary>
        /// Comprehensive business email evaluation.
        /// Detailed assessment of all professional communication aspects.
        /// </summary>
        public static string Accurate => """
                                         Evaluate whether this text represents a professional business email.

                                         Professional email criteria:
                                         - Structure: Clear subject line, greeting, body, and closing
                                         - Tone: Professional, respectful, and business-appropriate
                                         - Language: Proper grammar, spelling, and punctuation
                                         - Content: Relevant business communication
                                         - Clarity: Clear purpose and actionable information

                                         Accept:
                                         - Internal business communications
                                         - Client correspondence
                                         - Vendor communications
                                         - Professional inquiries and responses

                                         Reject:
                                         - Personal or casual messages
                                         - Spam or promotional content
                                         - Unprofessional language or tone
                                         - Unclear or confusing communications
                                         """;
    }

    /// <summary>
    /// Validates business proposal structure and content quality.
    /// Evaluates completeness, professionalism, and persuasiveness.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> Proposal review, business document validation, RFP responses</para>
    /// </remarks>
    public class ProposalValidation : IPromptTemplate
    {
        /// <summary>
        /// Quick business proposal format check.
        /// Basic validation for proposal structure.
        /// </summary>
        public static string Fast => "Must be business proposal format.";

        /// <summary>
        /// Standard business proposal validation with key components.
        /// Checks essential elements of effective proposals.
        /// </summary>
        public static string Balanced => """
                                         Check if this is a well-structured business proposal.

                                         Should include:
                                         - Clear problem statement or opportunity
                                         - Proposed solution or approach
                                         - Timeline or implementation plan
                                         - Budget or cost considerations
                                         """;

        /// <summary>
        /// Comprehensive business proposal evaluation.
        /// Detailed assessment of all proposal components and quality indicators.
        /// </summary>
        public static string Accurate => """
                                         Evaluate whether this text represents a comprehensive business proposal.

                                         Essential components:
                                         - Executive summary or introduction
                                         - Problem statement or business need
                                         - Proposed solution with clear benefits
                                         - Implementation timeline and milestones
                                         - Budget, costs, or resource requirements
                                         - Risk assessment or mitigation strategies
                                         - Clear next steps or call to action

                                         Quality indicators:
                                         - Professional presentation and structure
                                         - Data-driven arguments and justifications
                                         - Realistic timelines and budgets
                                         - Clear value proposition
                                         - Addresses potential concerns or objections
                                         """;
    }
}