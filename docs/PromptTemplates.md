# 📝 Prompt Templates

Pre-built templates for common validation scenarios with three quality levels: Fast, Balanced, Accurate.

## 📋 Template Types

- **Static**: `GrammarValidationPrompts.Fast`
- **Parameterized**: `TopicValidationPrompts.Fast("AI")`

### Quality Variants

Every template comes in three variants optimized for different needs:

| Variant | Token Usage | Speed | Accuracy | Best For |
|---------|-------------|--------|----------|----------|
| **Fast** | Low (20-50 tokens) | ⚡⚡⚡ | ⭐⭐ | High-volume processing, basic checks |
| **Balanced** | Medium (50-150 tokens) | ⚡⚡ | ⭐⭐⭐ | General use, good default choice |
| **Accurate** | High (150-300 tokens) | ⚡ | ⭐⭐⭐⭐⭐ | Critical content, detailed analysis |

### 🎯 How to Access Template Variants

You have two ways to access template variants:

#### 1. **Direct Access** (Recommended when variant is known)
```csharp
// Static templates (no parameters)
var fastGrammar = GrammarValidationPrompts.Fast;
var balancedGrammar = GrammarValidationPrompts.Balanced;
var accurateGrammar = GrammarValidationPrompts.Accurate;

// Parameterized templates
var fastTopic = TopicValidationPrompts.Fast("machine learning");
var balancedTone = ToneValidationPrompts.Balanced("professional");
var accurateContent = ContentRequirementValidationPrompt.Accurate("pricing and features");

// Nested business templates
var fastEmail = BusinessValidationPrompts.EmailValidation.Fast;
var accurateProposal = BusinessValidationPrompts.ProposalValidation.Accurate;
```

**Use direct access when:**
- ✅ You know the variant at compile time
- ✅ Performance is critical (no reflection overhead)
- ✅ Code is more readable and maintainable
- ✅ You want IntelliSense support

#### 2. **Extension Method** (For dynamic variant selection)
```csharp
// When variant is determined at runtime
var variant = GetVariantBasedOnUserTier(user.Tier); // Returns PromptVariant

// Static templates
var grammarPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(variant);

// Parameterized templates
var topicPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(variant, topic);
var tonePrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(variant, expectedTone);

// Business templates
var emailPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(variant);
```

**Use extension method when:**
- ✅ Variant is determined by business logic at runtime
- ✅ You need to support user-configurable quality levels
- ✅ Different conditions require different variants
- ✅ You're building a flexible validation pipeline

### 🔧 Practical Examples

#### Static Approach (Compile-Time Known)
```csharp
public class BlogPostValidator
{
    private readonly ILLMValidator _validator;

    public async Task<bool> ValidateForPublicationAsync(string content)
    {
        // We always want high accuracy for published content
        var result = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = GrammarValidationPrompts.Accurate, // Direct access
            ClientModelName = "gpt-4"
        });

        return result.IsValid;
    }

    public async Task<bool> ValidateUserCommentAsync(string comment)
    {
        // Fast check is sufficient for user comments
        var result = await _validator.ValidateAsync(comment, new LLMValidationOptions
        {
            ValidationPrompt = AppropriatenessValidationPrompts.Fast, // Direct access
            ClientModelName = "gpt-3.5-turbo"
        });

        return result.IsValid;
    }
}
```

#### Dynamic Approach (Runtime Selection)
```csharp
public class FlexibleContentValidator
{
    private readonly ILLMValidator _validator;

    public async Task<bool> ValidateContentAsync(string content, string category, UserTier userTier)
    {
        // Variant selection based on user tier and content importance
        var variant = (userTier, category) switch
        {
            (UserTier.Premium, "Legal") => PromptVariant.Accurate,
            (UserTier.Premium, _) => PromptVariant.Balanced,
            (UserTier.Basic, "Legal") => PromptVariant.Balanced,
            (UserTier.Basic, _) => PromptVariant.Fast,
            _ => PromptVariant.Fast
        };

        // Use extension method for dynamic variant selection
        var grammarPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(variant);
        var topicPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(variant, category);

        var grammarResult = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = grammarPrompt,
            ClientModelName = GetModelForVariant(variant)
        });

        if (!grammarResult.IsValid) return false;

        var topicResult = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = topicPrompt,
            ClientModelName = GetModelForVariant(variant)
        });

        return topicResult.IsValid;
    }

    private string GetModelForVariant(PromptVariant variant) => variant switch
    {
        PromptVariant.Fast => "gpt-3.5-turbo",
        PromptVariant.Balanced => "gpt-4",
        PromptVariant.Accurate => "gpt-4",
        _ => "gpt-3.5-turbo"
    };
}
```

## 🧠 System Prompts

System prompts establish the basic behavior and response format for LLM validation.

```csharp
using LLMValidation.Prompts;

// Basic system prompt - minimal instructions
SystemValidationPrompts.Fast
// → "You are a text validator. Respond with JSON only: {\"v\": boolean, \"r\": string|null}"

// Balanced system prompt - clear structure
SystemValidationPrompts.Balanced
// → Structured JSON format with clear rules

// Comprehensive system prompt - detailed guidelines
SystemValidationPrompts.Accurate
// → Complete validation framework with examples
```

### Usage in Configuration
```csharp
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4", options =>
    {
        options.SystemPrompt = SystemValidationPrompts.Accurate; // Detailed instructions
    })
    .AddModelOption("gpt-3.5-turbo", options =>
    {
        options.SystemPrompt = SystemValidationPrompts.Balanced; // Standard instructions
    });
```

## ✏️ Grammar & Language Validation

Validates text for grammar, spelling, and language quality.

### Templates Available

```csharp
using LLMValidation.Prompts;
using LLMValidation.Prompts.Abstraction;

// Quick grammar check
var fastPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Fast);
// → "Check grammar and spelling quickly."

// Standard grammar validation
var balancedPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced);
// → Structured prompt covering common grammar issues

// Comprehensive language analysis
var accuratePrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Accurate);
// → Detailed analysis including style and usage
```

### Real-World Example

```csharp
public class DocumentReviewService
{
    private readonly ILLMValidator _validator;

    public async Task<ValidationResult> ReviewDocumentAsync(string content, DocumentType type)
    {
        var variant = type switch
        {
            DocumentType.Email => PromptVariant.Fast,        // Quick check for emails
            DocumentType.BlogPost => PromptVariant.Balanced,  // Standard for blog posts
            DocumentType.PressRelease => PromptVariant.Accurate, // Thorough for press releases
            _ => PromptVariant.Balanced
        };

        var result = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(variant),
            ClientModelName = "gpt-4"
        });

        return new ValidationResult
        {
            IsValid = result.IsValid,
            Issues = result.Message,
            Confidence = variant == PromptVariant.Accurate ? "High" : "Medium"
        };
    }
}
```

## 🎭 Content Analysis Templates

### Topic Relevance Validation

Validates that content stays on topic and is relevant to the specified subject.

```csharp
// Parameterized template - requires topic argument
var topicPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(
    PromptVariant.Balanced,
    "machine learning"
);

// Usage example
var result = await validator.ValidateAsync(blogPost, new LLMValidationOptions
{
    ValidationPrompt = topicPrompt,
    ClientModelName = "gpt-4"
});
```

**Two Ways to Access Variants:**

```csharp
// 1. Direct access (compile-time known, more efficient)
var fastTopicPrompt = TopicValidationPrompts.Fast("AI");
// → "Must be about AI."

var balancedTopicPrompt = TopicValidationPrompts.Balanced("AI");
// → Checks focus, relevance, and basic alignment

var accurateTopicPrompt = TopicValidationPrompts.Accurate("AI");
// → Detailed evaluation including context and intent

// 2. Extension method (runtime variant selection)
var runtimeVariant = PromptVariant.Balanced; // Determined at runtime
var dynamicTopicPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(runtimeVariant, "AI");
```

### Tone Validation

Ensures content maintains the appropriate tone for its audience and purpose.

```csharp
// Professional tone check
var tonePrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(
    PromptVariant.Balanced,
    "professional"
);

// Friendly tone check
var friendlyPrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(
    PromptVariant.Fast,
    "friendly"
);

// Formal tone with comprehensive analysis
var formalPrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(
    PromptVariant.Accurate,
    "formal"
);
```

**Advanced Tone Examples:**

```csharp
public class CustomerServiceValidator
{
    public async Task<bool> ValidateResponseToneAsync(string response, string customerType)
    {
        var expectedTone = customerType switch
        {
            "VIP" => "respectful and accommodating",
            "Complaint" => "empathetic and solution-focused",
            "Technical" => "helpful and knowledgeable",
            _ => "professional and friendly"
        };

        var result = await _validator.ValidateAsync(response, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(
                PromptVariant.Balanced, expectedTone),
            ClientModelName = "gpt-4"
        });

        return result.IsValid;
    }
}
```

### Content Requirements Validation

Checks if content contains specific required information or elements.

```csharp
// Check for pricing information
var pricingPrompt = PromptTemplateExtensions.GetVariant<ContentRequirementValidationPrompt, string>(
    PromptVariant.Balanced,
    "pricing information and cost breakdown"
);

// Check for technical specifications
var specsPrompt = PromptTemplateExtensions.GetVariant<ContentRequirementValidationPrompt, string>(
    PromptVariant.Accurate,
    "technical specifications, system requirements, and compatibility details"
);
```

### Appropriateness Validation

Validates content for safety, appropriateness, and general audience suitability.

```csharp
// Quick appropriateness check
var quickCheck = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast);
// → "Must be appropriate and safe content."

// Standard safety validation
var standardCheck = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Balanced);
// → Covers common inappropriate content types

// Comprehensive content safety
var thoroughCheck = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Accurate);
// → Detailed evaluation with context consideration
```

## 🏢 Business Content Templates

### Professional Email Validation

Validates business email structure, tone, and professionalism.

```csharp
// Direct access when you know the variant (compile-time, more efficient)
var emailFast = BusinessValidationPrompts.EmailValidation.Fast;
// → "Must be professional business email."

var emailBalanced = BusinessValidationPrompts.EmailValidation.Balanced;
// → Checks tone, structure, and business appropriateness

var emailAccurate = BusinessValidationPrompts.EmailValidation.Accurate;
// → Detailed assessment of all professional communication aspects

// Extension method when variant is determined at runtime
var runtimeVariant = recipientType == "VIP" ? PromptVariant.Accurate : PromptVariant.Balanced;
var emailPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(runtimeVariant);
```

**Email Validation Pipeline:**

```csharp
public class EmailApprovalService
{
    public async Task<EmailApprovalResult> ValidateEmailAsync(string emailContent, EmailContext context)
    {
        var variant = context.Importance switch
        {
            EmailImportance.Internal => PromptVariant.Fast,
            EmailImportance.Customer => PromptVariant.Balanced,
            EmailImportance.Executive => PromptVariant.Accurate,
            _ => PromptVariant.Balanced
        };

        var result = await _validator.ValidateAsync(emailContent, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(variant),
            ClientModelName = context.Importance == EmailImportance.Executive ? "gpt-4" : "gpt-3.5-turbo",
            ErrorMessage = "Email does not meet company communication standards"
        });

        return new EmailApprovalResult
        {
            Approved = result.IsValid,
            Feedback = result.Message,
            RequiresReview = !result.IsValid && context.Importance == EmailImportance.Executive
        };
    }
}
```

### Business Proposal Validation

Validates business proposals for completeness and professional presentation.

```csharp
// Direct access when you know the variant at compile time (recommended)
var proposalFast = BusinessValidationPrompts.ProposalValidation.Fast;
var proposalBalanced = BusinessValidationPrompts.ProposalValidation.Balanced;
var proposalAccurate = BusinessValidationPrompts.ProposalValidation.Accurate;

// Extension method for dynamic variant selection at runtime
var dynamicVariant = GetVariantBasedOnBusinessLogic(); // Returns PromptVariant
var proposalPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.ProposalValidation>(dynamicVariant);
```

## 🔧 Custom Template Creation

You can create your own templates following the established patterns:

### Creating a Static Template

```csharp
using LLMValidation.Prompts.Abstraction;

namespace MyApp.Validation.Prompts;

/// <summary>
/// Validates code documentation quality and completeness.
/// </summary>
public class CodeDocumentationPrompts : IPromptTemplate
{
    /// <summary>
    /// Quick documentation check for basic requirements.
    /// </summary>
    public static string Fast => "Check if code has basic documentation.";

    /// <summary>
    /// Standard documentation validation with key quality factors.
    /// </summary>
    public static string Balanced => """
        Check if the code documentation is clear and helpful.

        Look for:
        - Clear descriptions of what the code does
        - Parameter explanations
        - Return value descriptions
        - Usage examples where appropriate
        """;

    /// <summary>
    /// Comprehensive documentation analysis with detailed criteria.
    /// </summary>
    public static string Accurate => """
        Evaluate the quality and completeness of code documentation.

        Documentation should include:
        - Clear, concise descriptions of functionality
        - Complete parameter documentation with types and purposes
        - Return value descriptions with expected formats
        - Exception documentation where relevant
        - Usage examples for complex methods
        - Links to related functionality where helpful

        Consider:
        - Clarity for developers at different skill levels
        - Consistency with established documentation patterns
        - Accuracy of technical details
        - Usefulness for API consumers
        """;
}
```

### Creating a Parameterized Template

```csharp
/// <summary>
/// Validates content against specific industry standards or guidelines.
/// </summary>
public class IndustryStandardsPrompts : IPromptTemplate<IndustryContext>
{
    public static string Fast(IndustryContext context) =>
        $"Must follow {context.Industry} standards.";

    public static string Balanced(IndustryContext context) => $"""
        Check if content follows {context.Industry} industry standards.

        Consider:
        - Industry-specific terminology and conventions
        - Required compliance elements for {context.Industry}
        - Professional standards and best practices
        """;

    public static string Accurate(IndustryContext context) => $"""
        Thoroughly evaluate content against {context.Industry} industry standards and regulations.

        Validation criteria:
        - Compliance with {context.Industry} regulatory requirements
        - Adherence to industry-specific terminology and conventions
        - Professional standards and ethical guidelines
        - Required disclosures and legal compliance for {context.Industry}

        {GetIndustrySpecificGuidelines(context.Industry)}

        Consider both mandatory requirements and best practices for the {context.Industry} industry.
        """;

    private static string GetIndustrySpecificGuidelines(string industry) => industry.ToLowerInvariant() switch
    {
        "healthcare" => "- HIPAA compliance and patient privacy\n- Medical accuracy and appropriate disclaimers\n- Professional medical communication standards",
        "finance" => "- SEC compliance and financial accuracy\n- Risk disclosures and investment disclaimers\n- Professional financial communication standards",
        "legal" => "- Legal accuracy and appropriate disclaimers\n- Professional legal communication standards\n- Confidentiality and privilege considerations",
        _ => "- Professional communication standards\n- Industry best practices\n- Regulatory compliance where applicable"
    };
}

public class IndustryContext
{
    public string Industry { get; set; } = string.Empty;
    public string[] RequiredElements { get; set; } = Array.Empty<string>();
    public ComplianceLevel ComplianceLevel { get; set; } = ComplianceLevel.Standard;
}

public enum ComplianceLevel
{
    Basic,
    Standard,
    Strict,
    Regulatory
}
```

### Using Custom Templates

```csharp
// Use your custom static template
var docPrompt = PromptTemplateExtensions.GetVariant<CodeDocumentationPrompts>(PromptVariant.Balanced);

var result = await validator.ValidateAsync(codeComment, new LLMValidationOptions
{
    ValidationPrompt = docPrompt,
    ClientModelName = "gpt-4"
});

// Use your custom parameterized template
var industryContext = new IndustryContext
{
    Industry = "Healthcare",
    RequiredElements = new[] { "HIPAA compliance", "medical disclaimers" },
    ComplianceLevel = ComplianceLevel.Regulatory
};

var industryPrompt = PromptTemplateExtensions.GetVariant<IndustryStandardsPrompts, IndustryContext>(
    PromptVariant.Accurate, industryContext);

var industryResult = await validator.ValidateAsync(medicalContent, new LLMValidationOptions
{
    ValidationPrompt = industryPrompt,
    ClientModelName = "gpt-4",
    Temperature = 0.0f // Very consistent for regulatory compliance
});
```

## 🎨 Template Design Best Practices

### 1. Progressive Complexity
Design your variants with increasing detail and specificity:

```csharp
// ✅ Good - Progressive complexity
Fast: "Check basic requirements"
Balanced: "Check requirements with context"
Accurate: "Comprehensive analysis with detailed criteria"

// ❌ Poor - Similar complexity
Fast: "Check all requirements thoroughly"
Balanced: "Check all requirements very thoroughly"
Accurate: "Check all requirements extremely thoroughly"
```

### 2. Token Optimization

Design prompts to minimize token usage while maximizing effectiveness:

```csharp
// ✅ Good - Efficient and clear
Fast: "Must be professional email."

// ❌ Poor - Unnecessarily verbose
Fast: "Please check if this text represents a professional business email communication."
```

### 3. Clear Instructions

Provide specific, actionable validation criteria:

```csharp
// ✅ Good - Specific criteria
Balanced: """
    Check if email is professional:
    - Appropriate greeting and closing
    - Clear subject and purpose
    - Business-appropriate language
    """;

// ❌ Poor - Vague criteria
Balanced: "Check if email is good and professional.";
```

### 4. Consistent Response Format

Ensure your templates work with the structured JSON response format:

```csharp
// Templates should focus on validation criteria
// The system prompt handles response format requirements

// ✅ Good - Focuses on what to validate
"Check if content is appropriate for general audiences"

// ❌ Poor - Tries to control response format
"Check if content is appropriate and respond with true/false"
```

## 📊 Template Performance Comparison

### Token Usage by Variant

| Template | Fast | Balanced | Accurate | Use Case |
|----------|------|----------|----------|----------|
| **Grammar** | 8 tokens | 45 tokens | 120 tokens | Language quality |
| **Topic** | 12 tokens | 65 tokens | 180 tokens | Content relevance |
| **Tone** | 10 tokens | 55 tokens | 150 tokens | Communication style |
| **Appropriateness** | 6 tokens | 85 tokens | 220 tokens | Content safety |
| **Business Email** | 8 tokens | 95 tokens | 250 tokens | Professional communication |

### Model Recommendations

| Template Category | GPT-3.5-turbo | GPT-4 | Local Models |
|-------------------|---------------|-------|--------------|
| **Grammar** | ✅ Fast/Balanced | ✅ All variants | ⚠️ Fast only |
| **Topic** | ✅ All variants | ✅ All variants | ✅ Fast/Balanced |
| **Tone** | ⚠️ Balanced only | ✅ All variants | ❌ Not recommended |
| **Appropriateness** | ✅ Fast/Balanced | ✅ All variants | ⚠️ Fast only |
| **Business** | ⚠️ Balanced only | ✅ All variants | ❌ Not recommended |

## 🔄 Integration with FluentValidation

All templates work seamlessly with FluentValidation extensions:

```csharp
public class ComprehensiveValidator : AbstractValidator<Document>
{
    public ComprehensiveValidator(ILLMValidator validator)
    {
        // Grammar validation
        RuleFor(x => x.Content)
            .MustHaveValidGrammar(validator, PromptVariant.Balanced);

        // Topic validation with dynamic topic
        RuleFor(x => x.Content)
            .MustBeAbout(validator, x => x.Category, PromptVariant.Accurate);

        // Tone validation based on document type
        RuleFor(x => x.Content)
            .MustHaveTone(validator, x => GetExpectedTone(x.Type), PromptVariant.Balanced);

        // Custom template usage
        RuleFor(x => x.Content)
            .MustPassLLMValidation(validator, options =>
            {
                options.ValidationPrompt = PromptTemplateExtensions.GetVariant<CodeDocumentationPrompts>(PromptVariant.Balanced);
                options.ClientModelName = "gpt-4";
            });
    }

    private string GetExpectedTone(DocumentType type) => type switch
    {
        DocumentType.UserManual => "helpful and instructional",
        DocumentType.Marketing => "engaging and persuasive",
        DocumentType.Technical => "precise and informative",
        _ => "professional"
    };
}
```

## 🚀 Next Steps

Now that you understand the prompt template system:

- **[🔄 FluentValidation Integration](FluentValidation.md)** - Learn advanced integration patterns
- **[🎭 Quality Variants](QualityVariants.md)** - Deep dive into variant selection
- **[⚙️ Configuration](Configuration.md)** - Configure templates for your needs
- **[🏢 Business Scenarios](BusinessScenarios.md)** - See templates in real-world contexts

---

**Master the Templates!** 🎯 The prompt template system is the foundation of effective LLM validation - choose the right template and variant for optimal results!