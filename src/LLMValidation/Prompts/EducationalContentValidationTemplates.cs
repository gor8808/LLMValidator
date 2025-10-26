using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Educational content validation templates.
/// Specialized prompts for learning materials, tutorials, and instructional content.
/// </summary>
public class EducationalContentValidationTemplates
{
    /// <summary>
    /// Validates educational lesson content structure and effectiveness.
    /// Evaluates learning objectives, organization, and pedagogical quality.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> Course content review, educational material validation, curriculum development</para>
    /// </remarks>
    public class LessonContent : IPromptTemplate
    {
        /// <summary>
        /// Quick educational content validation.
        /// Basic check for educational structure.
        /// </summary>
        public static string Fast => "Must be educational lesson content.";

        /// <summary>
        /// Standard educational content validation with key components.
        /// Checks essential elements of effective learning materials.
        /// </summary>
        public static string Balanced => """
                                         Check if this is structured educational content.

                                         Should include:
                                         - Clear learning objectives
                                         - Organized presentation of information
                                         - Examples or illustrations
                                         - Appropriate level for target audience
                                         """;

        /// <summary>
        /// Comprehensive educational content evaluation.
        /// Detailed assessment of pedagogical quality and learning effectiveness.
        /// </summary>
        public static string Accurate => """
                                         Evaluate the quality and structure of this educational content.

                                         Educational criteria:
                                         - Learning objectives: Clear goals and outcomes
                                         - Content organization: Logical flow and structure
                                         - Clarity: Appropriate language for target audience
                                         - Examples: Relevant illustrations or case studies
                                         - Engagement: Interactive or thought-provoking elements
                                         - Assessment: Questions, exercises, or review materials
                                         - Accuracy: Factual correctness and current information

                                         Quality indicators:
                                         - Progressive difficulty and concept building
                                         - Multiple learning modalities addressed
                                         - Practical application opportunities
                                         - Clear explanations of complex concepts
                                         - Inclusive and accessible presentation
                                         """;
    }
}