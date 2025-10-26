# 🚀 Quick Start

Get LLMValidator running in 5 minutes.

## 📦 Installation

```bash
dotnet add package LLMValidation
dotnet add package LLMValidation.FluentValidation
dotnet add package Microsoft.Extensions.AI.OpenAI
```

## ⚙️ Setup

**Program.cs:**
```csharp
// Register services
builder.Services.AddLLMValidator();
builder.Services.AddSingleton<IChatClient>(provider =>
    new OpenAIClient("your-api-key").AsChatClient("gpt-4"));
```

**Basic Usage:**
```csharp
[ApiController]
public class PostsController : ControllerBase
{
    private readonly ILLMValidator _validator;

    public PostsController(ILLMValidator validator) => _validator = validator;

    [HttpPost]
    public async Task<IActionResult> CreatePost(string content)
    {
        var result = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = "Check if this content is appropriate and has good grammar",
            ClientModelName = "gpt-4"
        });

        return result.IsValid ? Ok() : BadRequest(result.Message);
    }
}
```

## 🔧 Pre-built Templates

```csharp
// Use built-in templates
var result = await _validator.ValidateAsync(content, new LLMValidationOptions
{
    ValidationPrompt = GrammarValidationPrompts.Balanced,
    ClientModelName = "gpt-4"
});

// Or direct access for better performance
var prompt = GrammarValidationPrompts.Fast; // No reflection
var result = await _validator.ValidateAsync(content, new LLMValidationOptions
{
    ValidationPrompt = prompt,
    ClientModelName = "gpt-3.5-turbo"
});
```

## 🔄 FluentValidation

```csharp
public class BlogPostValidator : AbstractValidator<BlogPost>
{
    public BlogPostValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MustHaveValidGrammar(validator);

        RuleFor(x => x.Content)
            .MustBeAppropriate(validator)
            .MustHaveTone(validator, "professional");
    }
}
```

## 🚀 Next Steps

- **[📝 Prompt Templates](PromptTemplates.md)** - All available templates
- **[🎭 Quality Variants](QualityVariants.md)** - Speed vs accuracy
- **[🔄 FluentValidation](FluentValidation.md)** - Complete integration guide

## 🎭 Quality Variants Explained

Choose the right balance of speed, accuracy, and cost:

### ⚡ Fast - High Volume Processing
```csharp
// Perfect for user-generated content that needs quick moderation
RuleFor(x => x.Comment)
    .MustBeAppropriate(validator, PromptVariant.Fast);

// Uses minimal tokens, optimized for speed
// Best for: Comments, tags, short descriptions
```

### ⚖️ Balanced - General Purpose (Default)
```csharp
// Good balance for most business scenarios
RuleFor(x => x.BlogPost)
    .MustHaveValidGrammar(validator, PromptVariant.Balanced);

// Reasonable accuracy with acceptable performance
// Best for: Blog posts, emails, general content
```

### 🎯 Accurate - Critical Content
```csharp
// Maximum precision for important content
RuleFor(x => x.PressRelease)
    .MustHaveValidGrammar(validator, PromptVariant.Accurate, "gpt-4");

// Comprehensive analysis, higher token usage
// Best for: Legal documents, press releases, published articles
```

## 🌐 Configuration Examples

### Multiple Models Setup
```csharp
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4", options =>
    {
        options.Temperature = 0.1f;
        options.MaxTokens = 200;
        options.SystemPrompt = SystemValidationPrompts.Accurate; // More detailed system prompt
    })
    .AddModelOption("gpt-3.5-turbo", options =>
    {
        options.Temperature = 0.3f;
        options.MaxTokens = 100;
        options.SystemPrompt = SystemValidationPrompts.Fast; // Concise system prompt
    });

// Register different clients for different models
builder.Services.AddKeyedSingleton<IChatClient>("gpt-4", (provider, key) =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey).AsChatClient("gpt-4");
});

builder.Services.AddKeyedSingleton<IChatClient>("gpt-3.5-turbo", (provider, key) =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey).AsChatClient("gpt-3.5-turbo");
});
```

### Azure OpenAI Setup
```csharp
builder.Services.AddSingleton<IChatClient>(provider =>
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"];
    var apiKey = builder.Configuration["AzureOpenAI:ApiKey"];
    var deploymentName = builder.Configuration["AzureOpenAI:DeploymentName"];

    return new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
        .AsChatClient(deploymentName);
});
```

### Local Ollama Setup
```csharp
builder.Services.AddSingleton<IChatClient>(provider =>
{
    var endpoint = builder.Configuration["Ollama:Endpoint"]; // e.g., "http://localhost:11434"
    return new OllamaApiClient(new Uri(endpoint)).AsChatClient("llama3");
});
```

## 🔧 Common Patterns

### Email Validation Service
```csharp
public class EmailValidationService
{
    private readonly ILLMValidator _validator;

    public EmailValidationService(ILLMValidator validator)
    {
        _validator = validator;
    }

    public async Task<EmailValidationResult> ValidateEmailAsync(string emailContent, string recipientType)
    {
        // Different validation based on recipient
        var promptVariant = recipientType switch
        {
            "customer" => PromptVariant.Accurate,    // High standards for customers
            "internal" => PromptVariant.Balanced,    // Reasonable for internal communication
            "automated" => PromptVariant.Fast,       // Quick check for automated emails
            _ => PromptVariant.Balanced
        };

        var result = await _validator.ValidateAsync(emailContent, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(promptVariant),
            ClientModelName = "gpt-4",
            ErrorMessage = $"Email does not meet {recipientType} communication standards"
        });

        return new EmailValidationResult
        {
            IsValid = result.IsValid,
            Issues = result.Message,
            Suggestion = result.IsValid ? "Email looks good!" : "Please review and revise before sending"
        };
    }
}
```

### Content Moderation Pipeline
```csharp
public class ContentModerationPipeline
{
    private readonly ILLMValidator _validator;

    public ContentModerationPipeline(ILLMValidator validator)
    {
        _validator = validator;
    }

    public async Task<ModerationResult> ModerateContentAsync(string content)
    {
        // Step 1: Quick appropriateness check
        var appropriatenessResult = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
            ClientModelName = "gpt-3.5-turbo"
        });

        if (!appropriatenessResult.IsValid)
        {
            return ModerationResult.Rejected(appropriatenessResult.Message);
        }

        // Step 2: Grammar check for published content
        var grammarResult = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced),
            ClientModelName = "gpt-4"
        });

        return grammarResult.IsValid
            ? ModerationResult.Approved()
            : ModerationResult.NeedsReview(grammarResult.Message);
    }
}
```

## ✅ Verification

Test your setup with this simple verification:

```csharp
[HttpGet("test-validation")]
public async Task<IActionResult> TestValidation()
{
    var testText = "This is a professional email about our upcoming product launch.";

    var result = await _validator.ValidateAsync(testText, new LLMValidationOptions
    {
        ValidationPrompt = "Check if this text is professional and well-written",
        ClientModelName = "gpt-4"
    });

    return Ok(new
    {
        isValid = result.IsValid,
        message = result.Message,
        rawResponse = result.RawResponse
    });
}
```

Expected response:
```json
{
  "isValid": true,
  "message": null,
  "rawResponse": "{\"v\":true,\"r\":null}"
}
```

## 🚀 Next Steps

Now that you have LLMValidator running, explore these topics:

- **[📝 Prompt Templates](PromptTemplates.md)** - Discover all available validation templates
- **[⚙️ Configuration](Configuration.md)** - Deep dive into configuration options
- **[🎭 Quality Variants](QualityVariants.md)** - Learn when to use each variant

## ❗ Troubleshooting

### Common Issues

**"Unable to resolve IChatClient"**
```csharp
// Make sure you registered your LLM client
builder.Services.AddSingleton<IChatClient>(provider =>
    new OpenAIClient("your-api-key").AsChatClient("gpt-4"));
```

**"Request timeout"**
```csharp
// Increase timeout for slower models
.AddModelOption("gpt-4", options =>
{
    options.TimeoutMs = TimeSpan.FromMinutes(2);
});
```

**"Invalid JSON response"**
```csharp
// This usually indicates the model isn't following the response format
// Try using a more reliable model like GPT-4 or increase temperature slightly
.AddModelOption("your-model", options =>
{
    options.Temperature = 0.1f; // Very low for consistent JSON
});
```

---
