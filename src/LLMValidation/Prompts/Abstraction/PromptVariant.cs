namespace LLMValidation.Prompts.Abstraction;

/// <summary>
/// Defines the available quality variants for prompt templates.
/// Each variant represents a different balance of speed, accuracy, and detail in LLM validation prompts.
/// </summary>
public enum PromptVariant
{
    /// <summary>
    /// Speed-optimized variant with minimal token usage.
    /// Provides basic validation with concise prompts for high-throughput scenarios.
    /// </summary>
    /// <remarks>
    /// <para><strong>Best for:</strong></para>
    /// <list type="bullet">
    /// <item>High-volume processing requirements</item>
    /// <item>Cost-sensitive applications</item>
    /// <item>Simple validation scenarios</item>
    /// <item>Real-time validation with strict latency requirements</item>
    /// </list>
    /// <para><strong>Trade-offs:</strong> May sacrifice some accuracy for speed and cost efficiency.</para>
    /// </remarks>
    Fast,

    /// <summary>
    /// Balanced variant providing good accuracy with reasonable performance.
    /// This is the recommended default for most validation scenarios.
    /// </summary>
    /// <remarks>
    /// <para><strong>Best for:</strong></para>
    /// <list type="bullet">
    /// <item>General-purpose validation (recommended default)</item>
    /// <item>Production applications with moderate volume</item>
    /// <item>Applications requiring good accuracy without premium costs</item>
    /// <item>Most business scenarios and user-facing validations</item>
    /// </list>
    /// <para><strong>Trade-offs:</strong> Optimal balance between accuracy, speed, and cost.</para>
    /// </remarks>
    Balanced,

    /// <summary>
    /// Accuracy-optimized variant with comprehensive validation criteria.
    /// Provides maximum detail and precision for critical validation scenarios.
    /// </summary>
    /// <remarks>
    /// <para><strong>Best for:</strong></para>
    /// <list type="bullet">
    /// <item>Critical content validation</item>
    /// <item>Compliance and regulatory scenarios</item>
    /// <item>High-stakes business decisions</item>
    /// <item>Content that requires nuanced understanding</item>
    /// </list>
    /// <para><strong>Trade-offs:</strong> Higher token usage and processing time for maximum accuracy.</para>
    /// </remarks>
    Accurate
}