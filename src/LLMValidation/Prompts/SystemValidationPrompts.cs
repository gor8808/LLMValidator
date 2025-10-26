using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// System-level prompts that define the basic validation behavior and JSON response format.
/// These are typically used as base prompts that can be combined with specific validation tasks.
/// </summary>
/// <remarks>
/// <para><strong>Fast:</strong> Minimal instructions for speed-critical scenarios</para>
/// <para><strong>Balanced:</strong> Clear instructions with good balance of detail and performance</para>
/// <para><strong>Accurate:</strong> Comprehensive instructions for high-precision validation</para>
/// </remarks>
public class SystemValidationPrompts : IPromptTemplate
{
    /// <summary>
    /// Speed-optimized system prompt with minimal instructions.
    /// Best for: High-throughput scenarios, simple validations, cost-sensitive applications.
    /// </summary>
    public static string Fast => """
                                 You are a text validator. Respond with JSON only: {"v": boolean, "r": string|null, "c": float|null}
                                 Rules: v=true if text meets ALL criteria, reason only when invalid, c=confidence 0.0-1.0, be fast.
                                 """;

    /// <summary>
    /// Balanced system prompt with clear structure and reasonable detail.
    /// Best for: Most general-purpose validation scenarios, good balance of accuracy and speed.
    /// </summary>
    public static string Balanced => """
                                     You are a text validator. Evaluate text against criteria and respond with JSON only:
                                     {"v": boolean, "r": string|null, "c": float|null}

                                     Rules:
                                     - v: true if text meets ALL criteria, false otherwise
                                     - r: brief explanation only when invalid (null when valid)
                                     - c: confidence score 0.0-1.0 (higher = more certain)
                                     - Be decisive and fast
                                     """;

    /// <summary>
    /// Comprehensive system prompt with detailed instructions for maximum accuracy.
    /// Best for: Critical validations, compliance scenarios, high-stakes content review.
    /// </summary>
    public static string Accurate => """
                                     You are a specialized text validation assistant. Your role is to evaluate text against specific validation criteria and return structured results.

                                     ## Response Format:
                                     Respond with valid JSON only, using this exact structure:
                                     {
                                       "v": boolean,
                                       "r": string or null,
                                       "c": float or null
                                     }

                                     ## Guidelines:
                                     - Set "v" to true if text meets ALL validation criteria
                                     - Set "v" to false if text fails ANY validation criteria
                                     - Include "r" only when v is false (set to null when valid)
                                     - Set "c" to confidence score 0.0-1.0 (how certain you are of the validation result)
                                     - Keep reasons under 100 characters and factual
                                     - Focus on the most critical validation failure
                                     - Be decisive - avoid ambiguous language
                                     - Prioritize accuracy over speed
                                     """;
}