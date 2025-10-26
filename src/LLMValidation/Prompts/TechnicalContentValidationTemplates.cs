using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Technical content validation templates.
/// Specialized prompts for documentation, code comments, and technical writing.
/// </summary>
public static class TechnicalContentValidationTemplates
{
    /// <summary>
    /// Validates code documentation quality and completeness.
    /// Evaluates clarity, accuracy, and usefulness for developers.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> Code review, documentation standards, API documentation</para>
    /// </remarks>
    public class CodeDocumentation : IPromptTemplate
    {
        /// <summary>
        /// Quick code documentation validation.
        /// Basic check for documentation presence and clarity.
        /// </summary>
        public static string Fast => "Must be clear code documentation.";

        /// <summary>
        /// Standard code documentation validation with key elements.
        /// Checks essential components of helpful documentation.
        /// </summary>
        public static string Balanced => """
                                         Check if this is clear and helpful code documentation.

                                         Should include:
                                         - Clear explanation of what the code does
                                         - Parameter descriptions (if applicable)
                                         - Return value explanation (if applicable)
                                         - Usage examples or important notes
                                         """;

        /// <summary>
        /// Comprehensive code documentation evaluation.
        /// Detailed assessment of documentation quality and completeness.
        /// </summary>
        public static string Accurate => """
                                         Evaluate the quality and completeness of this code documentation.

                                         Documentation criteria:
                                         - Purpose: Clear explanation of what the code/function does
                                         - Parameters: Description of all parameters, their types, and usage
                                         - Return values: Explanation of what is returned and when
                                         - Examples: Practical usage examples when helpful
                                         - Edge cases: Important limitations, exceptions, or special cases
                                         - Clarity: Easy to understand for the target audience

                                         Quality indicators:
                                         - Accurate and up-to-date information
                                         - Comprehensive coverage of functionality
                                         - Clear, concise writing
                                         - Proper technical terminology
                                         - Helpful for developers using the code
                                         """;
    }

    /// <summary>
    /// Validates API documentation completeness and accuracy.
    /// Evaluates technical specifications, examples, and usability.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> API documentation review, developer experience, integration guides</para>
    /// </remarks>
    public class APIDocumentation : IPromptTemplate
    {
        /// <summary>
        /// Quick API documentation validation.
        /// Basic check for API documentation presence.
        /// </summary>
        public static string Fast => "Must be API documentation.";

        /// <summary>
        /// Standard API documentation validation with essential elements.
        /// Checks key components of functional API documentation.
        /// </summary>
        public static string Balanced => """
                                         Check if this is proper API documentation.

                                         Should include:
                                         - Endpoint or method description
                                         - Request/response format
                                         - Parameters and their requirements
                                         - Error codes or status information
                                         """;

        /// <summary>
        /// Comprehensive API documentation evaluation.
        /// Detailed assessment of all documentation aspects and quality.
        /// </summary>
        public static string Accurate => """
                                         Evaluate the completeness and quality of this API documentation.

                                         Required elements:
                                         - Endpoint URL and HTTP method
                                         - Clear description of functionality
                                         - Request format (headers, body, parameters)
                                         - Response format and structure
                                         - Status codes and error handling
                                         - Authentication requirements (if applicable)
                                         - Rate limiting information (if applicable)

                                         Quality criteria:
                                         - Complete and accurate technical details
                                         - Clear examples of requests and responses
                                         - Error scenarios and troubleshooting
                                         - Proper formatting and structure
                                         - Up-to-date with current API version
                                         """;
    }
}