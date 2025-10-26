# 🔄 FluentValidation Integration

LLMValidator provides **seamless integration** with FluentValidation, allowing you to combine traditional validation rules with intelligent LLM-based validation in a single, elegant fluent interface.

## 🚀 Quick Start Integration

### Basic Setup

```csharp
// Install packages
// dotnet add package LLMValidation.FluentValidation

using LLMValidation.FluentValidation;
using FluentValidation;

public class BlogPostValidator : AbstractValidator<BlogPost>
{
    public BlogPostValidator(ILLMValidator llmValidator)
    {
        // Traditional validation rules
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(10, 100);

        // LLM validation rules - seamlessly integrated!
        RuleFor(x => x.Content)
            .MustHaveValidGrammar(llmValidator)    // Check grammar and spelling
            .MustBeAppropriate(llmValidator)       // Check content safety
            .MustHaveTone(llmValidator, "professional"); // Check tone
    }
}
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register LLMValidator
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4", opt => opt.Temperature = 0.1f);

builder.Services.AddSingleton<IChatClient>(provider =>
    new OpenAIClient("your-api-key").AsChatClient("gpt-4"));
```

## 🎯 Core Extension Methods

### Grammar & Spelling Validation

```csharp
// Basic grammar validation with Balanced variant (default)
RuleFor(x => x.Content)
    .MustHaveValidGrammar(validator);

// Grammar validation with specific variant and model
RuleFor(x => x.Content)
    .MustHaveValidGrammar(validator, PromptVariant.Accurate, "gpt-4");
```

**Real-world Example:**
```csharp
public class DocumentValidator : AbstractValidator<Document>
{
    public DocumentValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MustHaveValidGrammar(validator, PromptVariant.Fast); // Quick check for titles

        RuleFor(x => x.Content)
            .MinimumLength(100)
            .MustHaveValidGrammar(validator, PromptVariant.Balanced); // Thorough for content

        // Conditional validation - stricter grammar for published content
        When(x => x.Status == DocumentStatus.Published, () =>
        {
            RuleFor(x => x.Content)
                .MustHaveValidGrammar(validator, PromptVariant.Accurate, "gpt-4")
                .WithMessage("Published content must have perfect grammar");
        });
    }
}
```

### Topic Relevance Validation

```csharp
// Static topic validation
RuleFor(x => x.Content)
    .MustBeAbout(validator, "technology");

// Dynamic topic validation based on object properties
RuleFor(x => x.Content)
    .MustBeAbout(validator, x => x.Category, PromptVariant.Accurate);

// Complex topic validation with custom logic
RuleFor(x => x.Content)
    .Must((obj, content) => ValidateTopicAsync(validator, content, obj.Tags).Result)
    .WithMessage("Content must be relevant to the specified topic");
```

**Advanced Topic Validation:**
```csharp
public class ArticleValidator : AbstractValidator<Article>
{
    public ArticleValidator(ILLMValidator validator)
    {
        // Multi-topic validation
        RuleFor(x => x.Content)
            .MustBeAbout(validator, x => string.Join(" or ", x.Tags), PromptVariant.Balanced)
            .When(x => x.Tags?.Length > 0)
            .WithMessage("Content should relate to at least one of the specified tags");

        // Category-specific topic validation
        RuleFor(x => x.Content)
            .MustBeAbout(validator, "technical tutorials with code examples", PromptVariant.Accurate)
            .When(x => x.Category == "Tutorial")
            .WithMessage("Tutorial articles must include technical content and code examples");
    }
}
```

### Tone & Style Validation

```csharp
// Simple tone validation
RuleFor(x => x.Email)
    .MustHaveTone(validator, "professional");

// Context-aware tone validation
RuleFor(x => x.Content)
    .MustHaveTone(validator, x => GetExpectedTone(x.Audience), PromptVariant.Balanced);
```

**Sophisticated Tone Validation:**
```csharp
public class CustomerCommunicationValidator : AbstractValidator<CustomerMessage>
{
    public CustomerCommunicationValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Subject)
            .MustHaveTone(validator, "professional and clear", PromptVariant.Fast);

        // Different tone requirements based on message type
        When(x => x.Type == MessageType.Complaint, () =>
        {
            RuleFor(x => x.Body)
                .MustHaveTone(validator, "empathetic and solution-focused", PromptVariant.Accurate)
                .WithMessage("Complaint responses must be empathetic and focus on solutions");
        });

        When(x => x.Type == MessageType.Sales, () =>
        {
            RuleFor(x => x.Body)
                .MustHaveTone(validator, "engaging and persuasive but not pushy", PromptVariant.Balanced)
                .WithMessage("Sales messages should be engaging but respectful");
        });

        When(x => x.Type == MessageType.Technical, () =>
        {
            RuleFor(x => x.Body)
                .MustHaveTone(validator, "helpful and knowledgeable", PromptVariant.Balanced)
                .WithMessage("Technical responses should be helpful and demonstrate expertise");
        });
    }
}
```

### Content Safety & Appropriateness

```csharp
// Basic appropriateness check
RuleFor(x => x.Comment)
    .MustBeAppropriate(validator);

// Strict appropriateness for public content
RuleFor(x => x.PublicPost)
    .MustBeAppropriate(validator, PromptVariant.Accurate)
    .WithMessage("Public content must meet the highest appropriateness standards");
```

### Content Requirements Validation

```csharp
// Check for specific content requirements
RuleFor(x => x.ProductDescription)
    .MustContainContent(validator, "pricing information and key features");

// Dynamic requirements based on context
RuleFor(x => x.JobPosting)
    .MustContainContent(validator, x => GetRequiredJobPostingElements(x.JobLevel), PromptVariant.Balanced);
```

## 🏢 Business-Specific Extensions

### Professional Email Validation

```csharp
public class EmailValidator : AbstractValidator<BusinessEmail>
{
    public EmailValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Subject)
            .NotEmpty()
            .Length(5, 100);

        // Professional email structure and tone validation
        RuleFor(x => x.Body)
            .MustBeProfessionalEmail(validator, PromptVariant.Balanced)
            .WithMessage("Email does not meet professional communication standards");

        // Extra validation for external emails
        When(x => x.IsExternalRecipient, () =>
        {
            RuleFor(x => x.Body)
                .MustBeProfessionalEmail(validator, PromptVariant.Accurate, "gpt-4")
                .WithMessage("External emails require the highest professional standards");
        });
    }
}
```

### Business Proposal Validation

```csharp
public class ProposalValidator : AbstractValidator<BusinessProposal>
{
    public ProposalValidator(ILLMValidator validator)
    {
        RuleFor(x => x.ExecutiveSummary)
            .NotEmpty()
            .MustBeBusinessProposal(validator, PromptVariant.Accurate)
            .WithMessage("Executive summary must follow business proposal structure");

        RuleFor(x => x.DetailedProposal)
            .MustContainContent(validator,
                "budget breakdown, timeline, deliverables, and risk assessment",
                PromptVariant.Accurate)
            .WithMessage("Proposal must include all required business elements");
    }
}
```

## 🔧 Advanced Integration Patterns

### Conditional LLM Validation

```csharp
public class SmartContentValidator : AbstractValidator<Content>
{
    public SmartContentValidator(ILLMValidator validator)
    {
        // Only validate with LLM if content is long enough to matter
        When(x => x.Text.Length > 100, () =>
        {
            RuleFor(x => x.Text)
                .MustHaveValidGrammar(validator, PromptVariant.Fast);
        });

        // Different validation based on user tier
        When(x => x.Author.Tier == UserTier.Premium, () =>
        {
            RuleFor(x => x.Text)
                .MustHaveTone(validator, "professional and authoritative", PromptVariant.Accurate);
        });

        When(x => x.Author.Tier == UserTier.Basic, () =>
        {
            RuleFor(x => x.Text)
                .MustBeAppropriate(validator, PromptVariant.Fast)
                .WithMessage("Content must be appropriate for public viewing");
        });
    }
}
```

### Multi-Step LLM Validation

```csharp
public class ComprehensiveArticleValidator : AbstractValidator<Article>
{
    public ComprehensiveArticleValidator(ILLMValidator validator)
    {
        // Step 1: Basic appropriateness (fast, must pass)
        RuleFor(x => x.Content)
            .MustBeAppropriate(validator, PromptVariant.Fast)
            .WithMessage("Content contains inappropriate material");

        // Step 2: Grammar check (only if appropriate)
        RuleFor(x => x.Content)
            .MustHaveValidGrammar(validator, PromptVariant.Balanced)
            .WithMessage("Content has grammar issues that need to be addressed")
            .DependentRules(() =>
            {
                // Step 3: Topic relevance (only if grammar is good)
                RuleFor(x => x.Content)
                    .MustBeAbout(validator, x => x.Category, PromptVariant.Balanced)
                    .WithMessage(x => $"Content should focus on {x.Category}");

                // Step 4: Professional tone (final check)
                RuleFor(x => x.Content)
                    .MustHaveTone(validator, "professional", PromptVariant.Balanced)
                    .WithMessage("Content should maintain a professional tone");
            });
    }
}
```

### Performance-Optimized Validation

```csharp
public class OptimizedValidator : AbstractValidator<UserPost>
{
    public OptimizedValidator(ILLMValidator validator, IConfiguration config)
    {
        // Choose variant based on system load or configuration
        var variant = GetOptimalVariant(config);
        var model = GetOptimalModel(config);

        RuleFor(x => x.Content)
            .MustBeAppropriate(validator, variant, model)
            .WithMessage("Content does not meet community standards");

        // Only do expensive validations during low-traffic hours
        When(x => IsLowTrafficHour(), () =>
        {
            RuleFor(x => x.Content)
                .MustHaveValidGrammar(validator, PromptVariant.Accurate, "gpt-4");
        });
    }

    private PromptVariant GetOptimalVariant(IConfiguration config)
    {
        var currentLoad = GetSystemLoad();
        return currentLoad switch
        {
            var load when load > 80 => PromptVariant.Fast,
            var load when load > 50 => PromptVariant.Balanced,
            _ => PromptVariant.Accurate
        };
    }
}
```

## 🔄 Custom Extensions

Create your own FluentValidation extensions for specific scenarios:

### Custom Extension Example

```csharp
public static class CustomLLMValidationExtensions
{
    /// <summary>
    /// Validates content for SEO-friendliness using LLM analysis
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeSEOFriendly<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        string targetKeywords,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = $"""
                Check if this content is SEO-friendly and optimized for the keywords: {targetKeywords}

                Evaluation criteria:
                - Natural integration of target keywords
                - Good readability and structure
                - Appropriate keyword density (not stuffing)
                - Clear headings and sections
                - Engaging and valuable content for readers
                """,
            ErrorMessage = $"Content is not optimized for SEO with keywords: {targetKeywords}"
        });
    }

    /// <summary>
    /// Validates content for accessibility and inclusive language
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeInclusive<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        ILLMValidator validator,
        PromptVariant variant = PromptVariant.Balanced,
        string? clientModelName = null)
    {
        return ruleBuilder.MustPassLLMValidation(validator, new LLMValidationOptions
        {
            ClientModelName = clientModelName ?? LLMValidationModelDefaultOption.DefaultClientName,
            ValidationPrompt = PromptTemplateExtensions.GetVariant<InclusiveLanguagePrompts>(variant),
            ErrorMessage = "Content contains non-inclusive language"
        });
    }
}

// Supporting prompt template
public class InclusiveLanguagePrompts : IPromptTemplate
{
    public static string Fast => "Check for inclusive, non-discriminatory language.";

    public static string Balanced => """
        Check if the content uses inclusive and accessible language.

        Look for:
        - Non-discriminatory language
        - Accessible terminology
        - Gender-neutral language where appropriate
        - Respectful references to different groups
        """;

    public static string Accurate => """
        Thoroughly evaluate the content for inclusive and accessible language.

        Evaluation criteria:
        - Avoids discriminatory language based on race, gender, age, ability, etc.
        - Uses person-first language when referring to disabilities
        - Employs gender-neutral language where appropriate
        - Avoids assumptions about audience demographics
        - Uses clear, jargon-free language for accessibility
        - Respects cultural and linguistic diversity

        Consider context and intent while promoting inclusivity.
        """;
}
```

### Using Custom Extensions

```csharp
public class InclusiveContentValidator : AbstractValidator<WebContent>
{
    public InclusiveContentValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Title)
            .MustBeSEOFriendly(validator, x => x.TargetKeywords, PromptVariant.Balanced)
            .MustBeInclusive(validator, PromptVariant.Fast);

        RuleFor(x => x.Body)
            .MustBeInclusive(validator, PromptVariant.Accurate)
            .WithMessage("Content must use inclusive and accessible language")
            .MustHaveValidGrammar(validator, PromptVariant.Balanced);

        When(x => x.IsPublicFacing, () =>
        {
            RuleFor(x => x.Body)
                .MustBeSEOFriendly(validator, x => x.TargetKeywords, PromptVariant.Accurate, "gpt-4")
                .WithMessage("Public content must be optimized for search engines");
        });
    }
}
```

## 🎯 Real-World Scenarios

### E-commerce Product Validation

```csharp
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MustContainContent(validator,
                "key features, benefits, and specifications",
                PromptVariant.Balanced)
            .WithMessage("Product description must include features, benefits, and specs")
            .MustHaveTone(validator, "engaging and informative", PromptVariant.Fast)
            .WithMessage("Product descriptions should be engaging");

        // Category-specific validation
        When(x => x.Category == "Electronics", () =>
        {
            RuleFor(x => x.Description)
                .MustContainContent(validator,
                    "technical specifications, compatibility, and warranty information",
                    PromptVariant.Accurate)
                .WithMessage("Electronics must include detailed technical information");
        });

        When(x => x.Category == "Clothing", () =>
        {
            RuleFor(x => x.Description)
                .MustContainContent(validator,
                    "size information, material details, and care instructions",
                    PromptVariant.Balanced)
                .WithMessage("Clothing must include size, material, and care information");
        });
    }
}
```

### Customer Support Ticket Validation

```csharp
public class SupportTicketValidator : AbstractValidator<SupportTicket>
{
    public SupportTicketValidator(ILLMValidator validator)
    {
        RuleFor(x => x.CustomerMessage)
            .NotEmpty()
            .MustBeAppropriate(validator, PromptVariant.Fast)
            .WithMessage("Message contains inappropriate content");

        RuleFor(x => x.AgentResponse)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.AgentResponse))
            .MustHaveTone(validator, "helpful and professional", PromptVariant.Balanced)
            .WithMessage("Agent response must be helpful and professional")
            .MustContainContent(validator, "specific solution or next steps", PromptVariant.Fast)
            .WithMessage("Response must include actionable solutions");

        // Escalated tickets need higher quality responses
        When(x => x.IsEscalated, () =>
        {
            RuleFor(x => x.AgentResponse)
                .MustHaveTone(validator, "empathetic and solution-focused", PromptVariant.Accurate, "gpt-4")
                .WithMessage("Escalated tickets require empathetic, solution-focused responses");
        });
    }
}
```

### Social Media Content Validation

```csharp
public class SocialMediaPostValidator : AbstractValidator<SocialPost>
{
    public SocialMediaPostValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(10, 280) // Twitter-like limit
            .MustBeAppropriate(validator, PromptVariant.Fast)
            .WithMessage("Content must be appropriate for public social media");

        // Platform-specific validation
        When(x => x.Platform == SocialPlatform.LinkedIn, () =>
        {
            RuleFor(x => x.Content)
                .MustHaveTone(validator, "professional", PromptVariant.Balanced)
                .WithMessage("LinkedIn posts should maintain a professional tone");
        });

        When(x => x.Platform == SocialPlatform.Twitter, () =>
        {
            RuleFor(x => x.Content)
                .MustHaveTone(validator, "engaging and concise", PromptVariant.Fast)
                .WithMessage("Twitter posts should be engaging and to the point");
        });

        // Brand account posts need higher standards
        When(x => x.IsBrandAccount, () =>
        {
            RuleFor(x => x.Content)
                .MustHaveTone(validator, "professional and on-brand", PromptVariant.Accurate)
                .WithMessage("Brand posts must align with company voice and values");
        });
    }
}
```

## 📊 Performance Considerations

### Optimization Strategies

```csharp
public class PerformanceOptimizedValidator : AbstractValidator<Content>
{
    public PerformanceOptimizedValidator(ILLMValidator validator, IMemoryCache cache)
    {
        // Cache results for repeated validations
        RuleFor(x => x.Text)
            .MustAsync(async (text, cancellation) =>
            {
                var cacheKey = $"validation:{text.GetHashCode()}";
                if (cache.TryGetValue(cacheKey, out bool cachedResult))
                {
                    return cachedResult;
                }

                var result = await validator.ValidateAsync(text, new LLMValidationOptions
                {
                    ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Fast),
                    ClientModelName = "gpt-3.5-turbo"
                });

                cache.Set(cacheKey, result.IsValid, TimeSpan.FromMinutes(30));
                return result.IsValid;
            })
            .WithMessage("Content validation failed");

        // Parallel validation for independent checks
        RuleFor(x => x.Text)
            .MustAsync(async (obj, text, cancellation) =>
            {
                var tasks = new[]
                {
                    validator.ValidateAsync(text, new LLMValidationOptions
                    {
                        ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
                        ClientModelName = "gpt-3.5-turbo"
                    }),
                    validator.ValidateAsync(text, new LLMValidationOptions
                    {
                        ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Fast),
                        ClientModelName = "gpt-3.5-turbo"
                    })
                };

                var results = await Task.WhenAll(tasks);
                return results.All(r => r.IsValid);
            })
            .WithMessage("Content failed multiple validation checks");
    }
}
```

## 🚀 Integration Best Practices

### ✅ Do's
- **Start with traditional validation** - Use FluentValidation for basic rules first
- **Layer LLM validation** - Add LLM validation as additional quality checks
- **Use appropriate variants** - Match variant to content importance and volume
- **Implement conditional validation** - Different rules for different scenarios
- **Cache results** - Avoid repeated validation of identical content
- **Handle failures gracefully** - Provide meaningful error messages

### ❌ Don'ts
- **Don't replace all validation with LLM** - Use LLM for complex, nuanced checks
- **Don't use Accurate variant everywhere** - It's expensive and often unnecessary
- **Don't ignore performance** - Monitor validation times and costs
- **Don't validate empty content with LLM** - Check for required content first
- **Don't forget error handling** - LLM calls can fail or timeout

## 🔧 Testing LLM-Integrated Validators

```csharp
[TestClass]
public class LLMValidationTests
{
    private IValidator<BlogPost> _validator;
    private Mock<ILLMValidator> _mockValidator;

    [TestInitialize]
    public void Setup()
    {
        _mockValidator = new Mock<ILLMValidator>();
        _validator = new BlogPostValidator(_mockValidator.Object);
    }

    [TestMethod]
    public async Task ValidateAsync_GoodContent_ShouldPass()
    {
        // Arrange
        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<string>(), It.IsAny<LLMValidationOptions>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(LLMValidationResult.Success());

        var post = new BlogPost
        {
            Title = "Great Article Title",
            Content = "This is well-written, appropriate content about technology.",
            Category = "Technology"
        };

        // Act
        var result = await _validator.ValidateAsync(post);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task ValidateAsync_InappropriateContent_ShouldFail()
    {
        // Arrange
        _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<string>(), It.IsAny<LLMValidationOptions>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(LLMValidationResult.Failure("Content contains inappropriate language"));

        var post = new BlogPost
        {
            Title = "Test Title",
            Content = "Inappropriate content here",
            Category = "Technology"
        };

        // Act
        var result = await _validator.ValidateAsync(post);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage.Contains("inappropriate")));
    }
}
```

## 🚀 Next Steps

Master FluentValidation integration:

- **[📝 Prompt Templates](PromptTemplates.md)** - Explore all available validation templates
- **[🎭 Quality Variants](QualityVariants.md)** - Optimize performance with smart variant selection
- **[⚙️ Configuration](Configuration.md)** - Fine-tune your validation setup

---

**Validate Intelligently!** 🔄 FluentValidation integration brings the power of LLM validation to your existing validation workflows with minimal code changes!