using LLMValidation.Prompts.Abstraction;

namespace LLMValidation.Prompts;

/// <summary>
/// Creative and marketing content validation templates.
/// Specialized prompts for marketing materials, product descriptions, and creative writing.
/// </summary>
public static class CreativeContentValidationTemplates
{
    /// <summary>
    /// Validates product description effectiveness for marketing purposes.
    /// Evaluates persuasiveness, clarity, and marketing appeal.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> E-commerce content, marketing copy, product launches</para>
    /// </remarks>
    public class ProductDescription : IPromptTemplate
    {
        /// <summary>
        /// Quick product description validation.
        /// Basic check for engaging marketing content.
        /// </summary>
        public static string Fast => "Must be engaging product description.";

        /// <summary>
        /// Standard product description validation with key marketing elements.
        /// Checks essential components of effective product marketing.
        /// </summary>
        public static string Balanced => """
                                         Check if this is an effective product description.

                                         Should include:
                                         - Key product features and benefits
                                         - Clear and engaging language
                                         - Target audience appeal
                                         - Call to action or purchasing information
                                         """;

        /// <summary>
        /// Comprehensive product description evaluation for marketing effectiveness.
        /// Detailed assessment of all marketing and persuasion aspects.
        /// </summary>
        public static string Accurate => """
                                         Evaluate the effectiveness of this product description for marketing purposes.

                                         Marketing criteria:
                                         - Product features: Clear description of what the product is/does
                                         - Benefits: How the product solves problems or adds value
                                         - Target audience: Language and appeal appropriate for buyers
                                         - Unique selling points: What makes this product special
                                         - Emotional appeal: Connects with customer needs and desires
                                         - Technical details: Relevant specifications when needed
                                         - Social proof: Testimonials, reviews, or endorsements
                                         - Call to action: Clear next steps for interested customers

                                         Quality indicators:
                                         - Compelling and persuasive language
                                         - Accurate product information
                                         - Professional presentation
                                         - SEO-friendly content (when applicable)
                                         - Brand-consistent messaging
                                         """;
    }

    /// <summary>
    /// Validates blog post quality and structure for content marketing.
    /// Evaluates engagement, organization, and value delivery.
    /// </summary>
    /// <remarks>
    /// <para><strong>Use cases:</strong> Content marketing, blog content review, publishing standards</para>
    /// </remarks>
    public class BlogPost : IPromptTemplate
    {
        /// <summary>
        /// Quick blog post validation.
        /// Basic check for blog content quality.
        /// </summary>
        public static string Fast => "Must be well-written blog post.";

        /// <summary>
        /// Standard blog post validation with key content elements.
        /// Checks essential components of effective blog content.
        /// </summary>
        public static string Balanced => """
                                         Check if this is a quality blog post.

                                         Should include:
                                         - Engaging title or headline
                                         - Clear introduction and conclusion
                                         - Organized main content
                                         - Appropriate tone for the topic
                                         """;

        /// <summary>
        /// Comprehensive blog post evaluation for content marketing effectiveness.
        /// Detailed assessment of all content quality and engagement aspects.
        /// </summary>
        public static string Accurate => """
                                         Evaluate the quality and structure of this blog post content.

                                         Blog post criteria:
                                         - Headline: Attention-grabbing and descriptive title
                                         - Introduction: Hook that draws readers in and sets expectations
                                         - Structure: Clear organization with headers, paragraphs, lists
                                         - Content quality: Valuable, informative, or entertaining information
                                         - Voice and tone: Consistent and appropriate for audience
                                         - Conclusion: Satisfying ending that reinforces main points
                                         - SEO elements: Keywords, meta descriptions, internal links
                                         - Engagement: Encourages comments, shares, or further reading

                                         Quality indicators:
                                         - Original, well-researched content
                                         - Scannable formatting with headers and bullet points
                                         - Relevant images, examples, or multimedia
                                         - Clear value proposition for readers
                                         - Professional writing and editing
                                         """;
    }
}