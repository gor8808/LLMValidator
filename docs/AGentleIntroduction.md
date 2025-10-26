# 🌟 Introduction to LLMValidator

LLMValidator adds intelligent validation to your .NET applications. It complements traditional validation by handling scenarios that require understanding meaning, context, and nuance.

## 🤔 What is LLM Validation?

Traditional validation excels at format, structure, and algorithmic checks - and these should remain your first line of defense. LLM validation handles the nuanced, contextual validation that traditional methods struggle with.

### Traditional Validation: Reliable Foundation
```csharp
// Email format validation - Perfect for this job
public bool IsValidEmailFormat(string email)
{
    var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    return Regex.IsMatch(email, pattern); // Fast, reliable, offline
}

// Required field validation - Simple and effective
public bool IsValidTitle(string title)
{
    return !string.IsNullOrWhiteSpace(title) && title.Length >= 5 && title.Length <= 100;
}
```

### Where Traditional Validation Struggles
```csharp
// Trying to validate professional tone traditionally - Complex and error-prone
public bool HasProfessionalTone(string text)
{
    var unprofessionalWords = new[] { "awesome", "cool", "dude", "whatever" };
    var sentences = text.Split('.');

    // Check for exclamation marks
    if (text.Count(c => c == '!') > sentences.Length * 0.1)
        return false;

    // Check for unprofessional words
    if (unprofessionalWords.Any(word =>
        text.Contains(word, StringComparison.OrdinalIgnoreCase)))
        return false;

    // ... dozens more rules, many edge cases, constant maintenance

    return true;
}
```

### LLM Validation: Handles Nuance
```csharp
// Professional tone validation with LLM - Natural and context-aware
var result = await validator.ValidateAsync(text, new LLMValidationOptions
{
    ValidationPrompt = "Check if this text has a professional, business-appropriate tone",
    ClientModelName = "gpt-4"
});

return result.IsValid;
```

### Best Practice: Layer Your Validation
```csharp
public async Task<ValidationResult> ValidateBusinessEmailAsync(string email, string subject, string body)
{
    // 1. Format validation first (fast, reliable)
    if (!IsValidEmailFormat(email))
        return ValidationResult.Fail("Invalid email format");

    if (!IsValidTitle(subject))
        return ValidationResult.Fail("Subject must be 5-100 characters");

    // 2. LLM validation for nuanced checks
    var professionalCheck = await validator.ValidateAsync(body, new LLMValidationOptions
    {
        ValidationPrompt = "Check if this email content is professional and appropriate for business communication",
        ClientModelName = "gpt-4"
    });

    if (!professionalCheck.IsValid)
        return ValidationResult.Fail($"Content issue: {professionalCheck.Message}");

    return ValidationResult.Success();
}
```

## 🚀 Core Concepts

### 📝 Prompt-Driven Validation

The heart of LLMValidator is **prompt-driven validation**. Instead of writing code, you describe what you want to validate in natural language.

```csharp
// Grammar and spelling
ValidationPrompt = "Check if the text has correct grammar and spelling"

// Content requirements
ValidationPrompt = "Ensure the text discusses pricing, features, and benefits"

// Tone and style
ValidationPrompt = "Verify the text maintains a friendly but professional tone"

// Business rules
ValidationPrompt = "Check if this email follows corporate communication standards"
```

### ⚡ Quality Variants

Every validation scenario offers three quality levels, each optimized for different needs:

| Variant | Speed | Accuracy | Cost | Best For |
|---------|-------|----------|------|----------|
| **Fast** | ⚡⚡⚡ | ⭐⭐ | 💰 | High-volume processing, basic checks |
| **Balanced** | ⚡⚡ | ⭐⭐⭐ | 💰💰 | Most scenarios, good default choice |
| **Accurate** | ⚡ | ⭐⭐⭐⭐⭐ | 💰💰💰 | Critical validation, detailed analysis |

```csharp
// Fast - for high-volume user comments
var result = await validator.ValidateAsync(comment, new LLMValidationOptions
{
    ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
    ClientModelName = "gpt-3.5-turbo"
});

// Accurate - for published articles
var result = await validator.ValidateAsync(article, new LLMValidationOptions
{
    ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Accurate),
    ClientModelName = "gpt-4"
});
```

### 🎯 Template System

LLMValidator comes with a comprehensive library of pre-built prompt templates:

#### 📚 **Grammar & Language**
```csharp
// Quick grammar check
PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Fast)
// → "Check grammar and spelling quickly."

// Comprehensive language analysis
PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Accurate)
// → Detailed prompt covering grammar, spelling, punctuation, and style
```

#### 🏢 **Business Content**
```csharp
// Professional email validation
PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(PromptVariant.Balanced)

// Business proposal structure
PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.ProposalValidation>(PromptVariant.Accurate)
```

#### 🎭 **Content Analysis**
```csharp
// Topic relevance (parameterized)
PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(PromptVariant.Balanced, "machine learning")

// Tone validation (parameterized)
PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(PromptVariant.Accurate, "professional")
```

## 🔧 How It Works Under the Hood

### 1. **Prompt Construction**
```csharp
// Your request
ValidationPrompt = "Check if this is appropriate for children"

// System adds context
SystemPrompt = "You are a text validator. Respond with JSON only: {\"v\": boolean, \"r\": string|null}"

// Combined message sent to LLM
// System: You are a text validator...
// User: Check if this is appropriate for children
// User: Text to validate: [your content]
```

### 2. **Structured Response**
```csharp
// LLM returns structured JSON
{
  "v": true,  // Is valid
  "r": null   // Reason (only when invalid)
}

// Or for invalid content
{
  "v": false,
  "r": "Contains inappropriate language for children"
}
```

### 3. **Result Processing**
```csharp
public class LLMValidationResult
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }      // Your custom message or LLM reason
    public string? RawResponse { get; set; }  // Full LLM response for debugging
}
```

## 🎯 Real-World Scenarios

### 📧 **Email Validation**
Instead of just checking format, validate the entire message:

```csharp
var emailResult = await validator.ValidateAsync(emailBody, new LLMValidationOptions
{
    ValidationPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(PromptVariant.Balanced),
    ClientModelName = "gpt-4",
    ErrorMessage = "Email does not meet professional communication standards"
});

// Validates: structure, tone, clarity, professionalism, completeness
```

### 📝 **Content Moderation**
Intelligent content filtering that understands context:

```csharp
var moderationResult = await validator.ValidateAsync(userPost, new LLMValidationOptions
{
    ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
    ClientModelName = "gpt-3.5-turbo"
});

// Understands intent, context, and nuanced appropriateness
```

### 📊 **Business Documents**
Validate complex business requirements:

```csharp
var proposalResult = await validator.ValidateAsync(proposal, new LLMValidationOptions
{
    ValidationPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.ProposalValidation>(PromptVariant.Accurate),
    ClientModelName = "gpt-4"
});

// Checks: structure, completeness, professional presentation, logical flow
```

## 🔄 Integration Patterns

### With FluentValidation
```csharp
public class BlogPostValidator : AbstractValidator<BlogPost>
{
    public BlogPostValidator(ILLMValidator llmValidator)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MustHaveValidGrammar(llmValidator, PromptVariant.Balanced);

        RuleFor(x => x.Content)
            .MustBeAbout(llmValidator, "technology", PromptVariant.Accurate)
            .MustHaveTone(llmValidator, "professional")
            .MustBeAppropriate(llmValidator);
    }
}
```

### With ASP.NET Core
```csharp
[ApiController]
public class ContentController : ControllerBase
{
    private readonly ILLMValidator _validator;

    public ContentController(ILLMValidator validator) => _validator = validator;

    [HttpPost("articles")]
    public async Task<IActionResult> CreateArticle(CreateArticleRequest request)
    {
        // Multi-step validation
        var grammarCheck = await _validator.ValidateAsync(request.Content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced),
            ClientModelName = "gpt-4"
        });

        if (!grammarCheck.IsValid)
            return BadRequest($"Grammar issues: {grammarCheck.Message}");

        var topicCheck = await _validator.ValidateAsync(request.Content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(
                PromptVariant.Accurate, request.Category),
            ClientModelName = "gpt-4"
        });

        if (!topicCheck.IsValid)
            return BadRequest($"Content not relevant to {request.Category}");

        // Continue with article creation...
        return Ok();
    }
}
```

## 🎨 Why This Approach Works

### **Context Understanding**
Traditional validation is perfect for patterns and formats, but LLM validation understands meaning and context:

```csharp
// Traditional validation: Great for format checking
if (Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
    // ✅ Correctly validates: john@company.com
    // ✅ Correctly rejects: invalid-email

// But traditional validation struggles with context
Text: "ur email is gr8, thx 4 the update!"
// ❌ Regex can't detect: informal tone, text speak, unprofessional style

// LLM validation understands context
Text: "This software helps kill processes efficiently"
// ✅ LLM understands: technical context, "kill" refers to terminating processes
// ❌ Simple word filtering would incorrectly flag "kill" as inappropriate

// Best approach: Use both
// 1. Regex validates format (fast, reliable)
// 2. LLM validates tone/appropriateness (contextual, nuanced)
```

### **Adaptability**
Requirements change, but LLM validation adapts:

```csharp
// Old requirement
ValidationPrompt = "Check if the email is professional"

// New requirement - no code changes needed
ValidationPrompt = "Check if the email is professional and includes a clear call-to-action"
```

### **Nuanced Analysis**
LLM validation captures subtleties that rule-based systems miss:

```csharp
// Tone analysis example
Text1: "Please review this document at your earliest convenience."
Text2: "You need to review this document ASAP."

// Both are requests, but different tones
// LLM correctly identifies: formal vs. urgent
```

## 🚀 Get Started

1. **[Quick Start](QuickStart.md)** - 5-minute setup
2. **[Prompt Templates](PromptTemplates.md)** - Available templates
3. **[FluentValidation](FluentValidation.md)** - Integration guide