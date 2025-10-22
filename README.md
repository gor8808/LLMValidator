# LLMValidator

### Semantic Validation for .NET using Large Language Models

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)

LLMValidator is a .NET library that brings the power of Large Language Models to your validation logic, enabling semantic validation that goes beyond traditional rule-based approaches.

---

## 💡 The Problem

Traditional validation is great for checking formats, ranges, and required fields. But what about validating:

- **Grammar and spelling** in user-generated content?
- **Topic relevance** - is this description actually about the subject?
- **Semantic content** - does the text contain the expected concepts?
- **Tone and style** - is the content appropriate for your use case?
- **Complex business rules** that are easier to express in natural language?

Writing rules for these scenarios is either impossible or requires massive complexity. But LLMs can understand context, meaning, and nuance.

## ✨ The Solution

LLMValidator bridges the gap between traditional validation and AI capabilities, allowing you to:

```csharp
RuleFor(x => x.Description)
    .MustHaveValidGrammar(llmValidator)
    .MustBeAbout(llmValidator, "dogs")
    .MustContain(llmValidator, "dog breed names");
```

That's it. No complex regex patterns, no massive dictionaries, no fragile rule engines. Just natural language validation that works.

---

## 🎯 Key Features

### 🔌 Universal Chat Client Support
Built on [Microsoft.Extensions.AI](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/), LLMValidator works with **any** chat client:
- ✅ OpenAI GPT models
- ✅ Azure OpenAI
- ✅ Anthropic Claude
- ✅ Local models via Ollama
- ✅ Google Gemini
- ✅ Any IChatClient implementation

### 🔗 Seamless FluentValidation Integration
Works naturally with FluentValidation's fluent API:
```csharp
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(ILLMValidator validator)
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MustHaveValidGrammar(validator)
            .MustBeAbout(validator, "the product category");
    }
}
```

### 🚀 Built-in Caching Support
Native integration with distributed caching to avoid redundant LLM calls:
```csharp
builder.AddOllamaApiClient("ollama")
    .AddChatClient()
    .UseDistributedCache();  // Automatic response caching
```

### 🎛️ Flexible Model Configuration
Use different models for different validation scenarios:
```csharp
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4")           // High accuracy
    .AddModelOption("gpt-3.5-turbo")   // Fast and cost-effective
    .AddModelOption("local-llama");     // Privacy-first
```

### 🏗️ Clean Architecture
- **No service locator anti-patterns** - dependency injection done right
- **Resolver pattern** for clean client resolution
- **Immutable options** for thread-safe configuration
- **Minimal abstractions** - only what you actually need

### ⚡ ASP.NET Core Ready
Automatic validation in your controllers:
```csharp
[HttpPost]
public IActionResult Create([FromBody] Product product)
{
    // FluentValidation with LLM rules runs automatically
    // This only executes if validation passes
    return Ok();
}
```

---

## 📦 Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **LLMValidation** | Core validation engine | [![NuGet](https://img.shields.io/badge/nuget-core-blue.svg)](https://www.nuget.org/packages/LLMValidation/) |
| **LLMValidation.FluentValidation** | FluentValidation integration with helper extensions | [![NuGet](https://img.shields.io/badge/nuget-fluentvalidation-blue.svg)](https://www.nuget.org/packages/LLMValidation.FluentValidation/) |

---

## 🚀 Quick Start

### 1️⃣ Installation

```bash
dotnet add package LLMValidation
dotnet add package LLMValidation.FluentValidation
dotnet add package FluentValidation.AspNetCore
```

### 2️⃣ Register Services

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add your chat client (example with Ollama for local models)
builder.AddOllamaApiClient("ollama")
    .AddChatClient()
    .UseDistributedCache();  // Optional: enable caching

// Or with OpenAI
// builder.Services.AddSingleton<IChatClient>(
//     new OpenAI.OpenAIClient(apiKey).AsChatClient("gpt-4"));

// Register LLM validator
builder.Services.AddLLMValidator();

// Add FluentValidation with automatic validation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
```

### 3️⃣ Create Your Validator

```csharp
using FluentValidation;
using LLMValidation.FluentValidation;

public class DogDescriptionValidator : AbstractValidator<DogDescription>
{
    public DogDescriptionValidator(ILLMValidator llmValidator)
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MustHaveValidGrammar(llmValidator)
            .MustBeAbout(llmValidator, "dogs")
            .MustContain(llmValidator, "dog breed names or dog-related terminology");
    }
}
```

### 4️⃣ Use in Your Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class DogsController : ControllerBase
{
    [HttpPost]
    public IActionResult Validate([FromBody] DogDescription model)
    {
        // Validation happens automatically before entering this method!
        // If validation fails, ModelState contains errors
        return Ok(new { IsValid = true });
    }
}
```

That's it! Your API now validates content semantically using LLMs.

---

## 📚 Use Cases

### ✍️ Content Moderation
Validate user-generated content for appropriateness, tone, and relevance:
```csharp
RuleFor(x => x.Comment)
    .MustHaveValidGrammar(validator)
    .MustBeAppropriate(validator)
    .MustBeAbout(validator, "the article topic");
```

### 📝 Form Validation
Ensure form inputs make sense semantically:
```csharp
RuleFor(x => x.CompanyDescription)
    .MustBeAbout(validator, "business activities and services")
    .MustContain(validator, "specific industry terminology")
    .MustHaveTone(validator, "professional");
```

### 🔍 Product Descriptions
Validate e-commerce content quality:
```csharp
RuleFor(x => x.ProductDescription)
    .MustHaveValidGrammar(validator)
    .MustContain(validator, "product features and specifications")
    .MustHaveTone(validator, "professional and engaging");
```

### 🎓 Educational Applications
Validate student submissions:
```csharp
RuleFor(x => x.Essay)
    .MustHaveValidGrammar(validator)
    .MustBeAbout(validator, "the assigned topic")
    .MustContain(validator, "supporting evidence and citations");
```

### 🏢 Business Rule Validation
Express complex business rules naturally:
```csharp
RuleFor(x => x.ProposalText)
    .MustPassLLMValidation(validator, new LLMValidationOptions
    {
        ValidationPrompt = @"Check if this proposal addresses all requirements:
            - Budget justification
            - Timeline with milestones
            - Risk assessment
            - Stakeholder analysis",
        ClientModelName = "gpt-4"  // Use high-accuracy model for critical validations
    });
```

---

## 💾 Caching Support

LLMValidator supports distributed caching at the IChatClient level using Microsoft.Extensions.AI's built-in caching middleware:

```csharp
// Add distributed cache (required for caching to work)
builder.Services.AddDistributedMemoryCache();
// Or use Redis: builder.Services.AddStackExchangeRedisCache(...)
// Or SQL Server: builder.Services.AddDistributedSqlServerCache(...)

// Enable caching for your chat client
builder.AddOllamaApiClient("ollama")
    .AddChatClient()
    .UseDistributedCache();  // Caching middleware
```

**Benefits:**
- ✅ Identical requests return cached responses instantly (no LLM call)
- ✅ Reduces LLM API costs significantly
- ✅ Improves response times (cached: ~1ms vs LLM: ~500ms+)
- ✅ Works transparently - no code changes in validators
- ✅ Supports any IDistributedCache implementation (Memory, Redis, SQL Server, etc.)

**How it works:**
The cache key is automatically generated based on:
- Complete prompt content
- Model name
- All LLM options (temperature, max tokens, etc.)
- System prompt

This means only truly identical validation requests are cached, ensuring correctness.

**Example with Redis for production:**
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "LLMValidator:";
});

builder.AddOllamaApiClient("ollama")
    .AddChatClient()
    .UseDistributedCache();  // Now backed by Redis
```

---

## 🔧 Configuration Options

### Model-Specific Configuration
Configure default options per model:
```csharp
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4", options =>
    {
        options.MaxTokens = 1000;
        options.Temperature = 0.1f;  // More deterministic
        options.SystemPrompt = "You are a precise validation assistant.";
    })
    .AddModelOption("gpt-3.5-turbo", options =>
    {
        options.MaxTokens = 500;
        options.Temperature = 0.0f;  // Fully deterministic
    });
```

### Validation-Specific Options
Override settings for specific validations:
```csharp
RuleFor(x => x.Description)
    .MustPassLLMValidation(validator, new LLMValidationOptions
    {
        ValidationPrompt = "Check if this is professional business communication",
        ClientModelName = "gpt-4",          // Use specific model
        Temperature = 0.0f,                  // Override temperature
        MaxTokens = 500,                     // Override max tokens
        ErrorMessage = "Content must be professional"
    });
```

### Built-in Helper Methods
All helper methods support optional model selection:
```csharp
RuleFor(x => x.CriticalField)
    .MustHaveValidGrammar(validator, clientModelName: "gpt-4");

RuleFor(x => x.SimpleField)
    .MustHaveValidGrammar(validator, clientModelName: "gpt-3.5-turbo");
```

---

## 🏗️ Architecture & Integration

### Universal Client Support

LLMValidator is built on **Microsoft.Extensions.AI**, which provides a unified `IChatClient` interface that works with any LLM provider:

```csharp
// OpenAI
builder.Services.AddSingleton<IChatClient>(
    new OpenAI.OpenAIClient(apiKey).AsChatClient("gpt-4"));

// Azure OpenAI
builder.AddAzureOpenAIClient("azureOpenAI");

// Ollama (local models)
builder.AddOllamaApiClient("ollama");

// Anthropic Claude
builder.Services.AddSingleton<IChatClient>(
    new Anthropic.AnthropicClient(apiKey).AsChatClient("claude-3-opus"));

// Google Gemini
builder.AddGoogleAIClient("gemini");
```

All of these work seamlessly with LLMValidator - **no provider-specific code needed**!

### Multiple Models with Keyed Services

Use different models for different validation scenarios using keyed services:

```csharp
// Register multiple models as keyed services
builder.AddOllamaApiClient("ollama-fast-connection")
    .AddKeyedChatClient("fast-model")
    .UseDistributedCache();

builder.AddOllamaApiClient("ollama-accurate-connection")
    .AddKeyedChatClient("accurate-model")
    .UseDistributedCache();

// Configure LLMValidator with both models
builder.Services.AddLLMValidator()
    .AddModelOption("fast-model")      // For simple, quick validations
    .AddModelOption("accurate-model"); // For complex, critical validations
```

Then specify which model to use in your validation:
```csharp
// Use fast model for simple checks
RuleFor(x => x.SimpleField)
    .MustHaveValidGrammar(validator, clientModelName: "fast-model");

// Use accurate model for critical validations
RuleFor(x => x.ComplexField)
    .MustPassLLMValidation(validator, new LLMValidationOptions
    {
        ClientModelName = "accurate-model",
        ValidationPrompt = "Validate complex business rule..."
    });
```

### Clean Resolver Pattern

LLMValidator uses a clean resolver pattern instead of service locator anti-patterns. The resolver handles getting the right `IChatClient` based on the model name:

```csharp
public interface IChatClientResolver
{
    IChatClient Resolve(IServiceProvider serviceProvider, string? modelName);
}
```

The default resolver:
- Returns keyed service if `modelName` is provided
- Returns default service if `modelName` is null or empty

Want custom resolution logic? Implement your own:
```csharp
public class CustomResolver : IChatClientResolver
{
    public IChatClient Resolve(IServiceProvider serviceProvider, string? modelName)
    {
        return modelName switch
        {
            "premium" => serviceProvider.GetRequiredKeyedService<IChatClient>("gpt-4"),
            "standard" => serviceProvider.GetRequiredKeyedService<IChatClient>("gpt-3.5"),
            _ => serviceProvider.GetRequiredService<IChatClient>()
        };
    }
}

// Register custom resolver
builder.Services.AddLLMValidator()
    .UseChatClientResolver<CustomResolver>();
```

### Immutable Options Pattern

Configuration options are immutable by design for thread-safety:

```csharp
// Options are merged with defaults, returning a new instance
var effectiveOptions = userOptions.WithDefaults(defaultOptions);

// Original options remain unchanged (immutable)
```

This ensures:
- Thread-safe configuration
- No unexpected side effects
- Predictable behavior in concurrent scenarios

---

## 🔬 Examples

The repository includes comprehensive examples demonstrating different use cases:

### 📊 Console Benchmarking App
Compare multiple LLM models for validation accuracy and performance:

**Location:** `examples/LLMValidation.Example.ConsoleApp`

Features:
- Benchmarks 4 different Ollama models simultaneously
- Runs comprehensive test suites (grammar, topic, content validation)
- Measures accuracy percentage and response times
- Displays winner with detailed metrics
- Demonstrates keyed services with multiple models
- Shows caching in action

**Run it:**
```bash
cd examples/LLMValidation.Example.AppHost
dotnet run
```

**Sample output:**
```
=============================================================================
BENCHMARK RESULTS
=============================================================================

Model: llama3.2:3b
  Accuracy:           90.91% (10/11 correct)
  Avg Response Time:  450ms
  Min Response Time:  280ms
  Max Response Time:  720ms

Model: gemma2:2b
  Accuracy:           81.82% (9/11 correct)
  Avg Response Time:  520ms
  ...

=============================================================================
🏆 WINNER: llama3.2:3b with 90.91% accuracy and 450ms avg response time
=============================================================================
```

### 🌐 ASP.NET Core API Example
Full-featured REST API with automatic semantic validation:

**Location:** `examples/LLMValidation.Example.Api`

Features:
- RESTful endpoints with automatic validation
- Swagger/OpenAPI documentation
- FluentValidation integration
- Proper MVC architecture (Controllers, Models, Validators)
- Demonstrates real-world API structure

**Run it:**
```bash
cd examples/LLMValidation.Example.AppHost
dotnet run
```

Then visit: `http://localhost:5000` for Swagger UI

**Example request:**
```bash
curl -X POST http://localhost:5000/api/validation \
  -H "Content-Type: application/json" \
  -d '{"description": "Golden Retrievers are friendly dogs."}'
```

### 🚀 .NET Aspire Orchestration
Both examples run with .NET Aspire for easy orchestration:

**Location:** `examples/LLMValidation.Example.AppHost`

Features:
- Automatic Ollama container setup
- Multiple model pre-loading and management
- Service discovery and configuration
- Aspire Dashboard for monitoring all services
- One command to run everything

**What it does:**
1. Starts Ollama container
2. Pulls 4 different models (llama3.2:3b, gemma2:2b, etc.)
3. Starts the API service
4. Starts the benchmarking console app
5. Wires everything together with proper connection strings
6. Opens Aspire Dashboard for monitoring

**Run everything:**
```bash
cd examples/LLMValidation.Example.AppHost
dotnet run
```

The Aspire Dashboard will open automatically, showing all running services and their health status.

---

## 🧪 Testing

The library includes comprehensive unit tests covering all functionality:

**Test Framework:**
- **xUnit** - Modern test framework
- **AutoFixture** - Automatic test data generation
- **AutoFixture.AutoMoq** - Automatic mocking
- **Moq** - Mocking framework
- **FluentAssertions** - Readable, expressive assertions

**Coverage includes:**
- Core validator logic
- Options merging and immutability
- Resolver pattern (default and custom)
- FluentValidation extensions
- Error handling and edge cases

**Run tests:**
```bash
cd tests/LLMValidation.Tests
dotnet test
```

**Example test:**
```csharp
[Fact]
public void WithDefaults_ShouldNotMutateOriginal()
{
    // Arrange
    var original = _fixture.Create<LLMValidationOptions>();
    var originalCopy = new LLMValidationOptions { /* ... */ };
    var defaults = _fixture.Create<LLMValidationModelDefaultOption>();

    // Act
    var result = original.WithDefaults(defaults);

    // Assert
    original.Should().BeEquivalentTo(originalCopy);  // Original unchanged
    result.Should().NotBeSameAs(original);           // New instance
}
```

---

## 🎓 Best Practices

### ♻️ Use Caching
Always enable caching for production to reduce costs and improve performance:
```csharp
builder.Services.AddDistributedMemoryCache();  // Or Redis for production
builder.AddOllamaApiClient("ollama")
    .AddChatClient()
    .UseDistributedCache();
```

### 🎯 Choose the Right Model
- **Simple validations** (grammar, spelling): Use small, fast models (llama3.2:1b, gpt-3.5-turbo)
- **Complex validations** (business rules, compliance): Use larger, accurate models (llama3.2:3b, gpt-4)
- **Privacy-sensitive**: Use local models via Ollama

### 🌡️ Temperature Settings
- **Validation tasks**: Use low temperature (0.0 - 0.2) for consistency
- **Default**: 0.3 provides good balance
- **Creative tasks**: Higher temperatures if needed, but not recommended for validation

### ⏱️ Set Timeouts
Always set appropriate timeouts based on your requirements:
```csharp
options.TimeoutMs = 10000;  // 10 seconds for critical path validations
```

### 🎨 Custom Error Messages
Provide clear, actionable error messages:
```csharp
RuleFor(x => x.Description)
    .MustHaveValidGrammar(validator)
    .WithMessage("Please check your description for grammar and spelling errors.");
```

### 📝 Test Your Prompts
Validation prompts should be clear and specific:
```csharp
// ✅ Good: Specific and clear
ValidationPrompt = "Check if the text is about dogs, mentions at least one dog breed, and uses proper grammar."

// ❌ Bad: Vague
ValidationPrompt = "Check if this is good."
```

### 🔄 Validate Critical Fields Only
LLM validation is slower than traditional validation (100ms - 2s):
- Use traditional validation for simple checks (NotEmpty, Length, Regex)
- Reserve LLM validation for fields that truly need semantic understanding

---

## 📖 API Reference

### Pre-built Validation Methods

#### MustHaveValidGrammar
Validates grammar and spelling:
```csharp
RuleFor(x => x.Description)
    .MustHaveValidGrammar(llmValidator, clientModelName: "gpt-4");
```

#### MustBeAbout
Validates topic relevance:
```csharp
RuleFor(x => x.Description)
    .MustBeAbout(llmValidator, "technology products", clientModelName: "gpt-4");
```

#### MustContain
Validates specific content presence:
```csharp
RuleFor(x => x.Description)
    .MustContain(llmValidator, "pricing information", clientModelName: "gpt-4");
```

#### MustHaveTone
Validates text tone:
```csharp
RuleFor(x => x.Description)
    .MustHaveTone(llmValidator, "professional", clientModelName: "gpt-4");
```

#### MustBeAppropriate
Validates content appropriateness:
```csharp
RuleFor(x => x.Comment)
    .MustBeAppropriate(llmValidator, clientModelName: "gpt-4");
```

#### MustPassLLMValidation
Custom validation with full control:
```csharp
RuleFor(x => x.Field)
    .MustPassLLMValidation(llmValidator, new LLMValidationOptions
    {
        ValidationPrompt = "Your custom validation logic here",
        ClientModelName = "gpt-4",
        Temperature = 0.0f,
        MaxTokens = 500
    });
```

### LLMValidationOptions

```csharp
public class LLMValidationOptions
{
    public string? ValidationPrompt { get; set; }
    public string? ClientModelName { get; set; }
    public string? SystemPrompt { get; set; }
    public int? MaxTokens { get; set; }
    public float? Temperature { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
```

---

## 🤝 Contributing

Contributions are welcome! Whether it's:
- 🐛 Bug reports
- ✨ Feature requests
- 📖 Documentation improvements
- 🧪 Test coverage
- 💡 New validation helpers

Please feel free to submit issues and pull requests.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

Built with amazing .NET technologies:

- **[Microsoft.Extensions.AI](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/)** - Unified AI abstractions that make provider-agnostic LLM integration possible
- **[FluentValidation](https://github.com/FluentValidation/FluentValidation)** - The most elegant validation library for .NET
- **[.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)** - Cloud-native development stack for .NET

Special thanks to the .NET community for continuous innovation and support.

---

<div align="center">

**[Getting Started](#-quick-start)** • **[Examples](#-examples)** • **[Use Cases](#-use-cases)** • **[Architecture](#-architecture--integration)**

Made with ❤️ for the .NET community

</div>
