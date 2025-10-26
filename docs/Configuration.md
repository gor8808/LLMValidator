# ⚙️ Configuration Guide

## 🚀 Microsoft.Extensions.AI: The Foundation

LLMValidator is built on **Microsoft.Extensions.AI**, a powerful abstraction that makes AI accessible to developers without requiring deep infrastructure knowledge. This groundbreaking package transforms AI integration from complex infrastructure challenges into simple, one-line additions to your application.

### 🎯 What Microsoft.Extensions.AI Enables

Microsoft.Extensions.AI provides:
- **Universal AI abstraction** - One interface for all providers
- **Seamless integration** - Works with OpenAI, Azure, Anthropic, local models
- **Structured responses** - Built-in JSON schema support
- **Dependency injection ready** - First-class DI support
- **Extensible architecture** - Easy to add new providers

```csharp
// One line to add AI validation to your app
builder.Services.AddSingleton<IChatClient>(provider =>
    new OpenAIClient("api-key").AsChatClient("gpt-4"));
```

### 🔧 Supported Providers

| Provider | Package | Usage |
|----------|---------|-------|
| **OpenAI** | `Microsoft.Extensions.AI.OpenAI` | `new OpenAIClient().AsChatClient()` |
| **Azure OpenAI** | `Microsoft.Extensions.AI.AzureAIInference` | `new AzureOpenAIClient().AsChatClient()` |
| **Anthropic** | `Microsoft.Extensions.AI.Anthropic` | `new AnthropicClient().AsChatClient()` |
| **Ollama** | `Microsoft.Extensions.AI.Ollama` | `new OllamaApiClient().AsChatClient()` |

## 🏗️ Dependency Injection Extensibility

LLMValidator is designed for maximum extensibility through dependency injection:

### Multiple Provider Setup

```csharp
// Register multiple providers with keys
builder.Services.AddKeyedSingleton<IChatClient>("openai", (provider, key) =>
    new OpenAIClient("openai-key").AsChatClient("gpt-4"));

builder.Services.AddKeyedSingleton<IChatClient>("anthropic", (provider, key) =>
    new AnthropicClient("anthropic-key").AsChatClient("claude-3"));

builder.Services.AddKeyedSingleton<IChatClient>("ollama", (provider, key) =>
    new OllamaApiClient(new Uri("http://localhost:11434")).AsChatClient("llama3"));

// Configure model-specific options
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4", opt => {
        opt.Temperature = 0.1f;
        opt.MaxTokens = 200;
    })
    .AddModelOption("claude", opt => {
        opt.Temperature = 0.0f;
        opt.MaxTokens = 150;
    });
```

## 📝 AI Chat Configuration Fields

### Core Configuration Options

```csharp
public class LLMValidationOptions
{
    /// <summary>The prompt describing what to validate</summary>
    public string ValidationPrompt { get; set; }

    /// <summary>Model name/key for client resolution</summary>
    public string? ClientModelName { get; set; }

    /// <summary>Custom error message when validation fails</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>System prompt for model behavior</summary>
    public string? SystemPrompt { get; set; }

    /// <summary>Controls randomness (0.0-2.0). Lower = more consistent</summary>
    public float? Temperature { get; set; }

    /// <summary>Maximum tokens in response. Lower = faster/cheaper</summary>
    public int? MaxTokens { get; set; }

    /// <summary>Minimum confidence threshold (0.0-1.0). Validation fails if LLM confidence is below this value</summary>
    public float? MinConfidence { get; set; }

    /// <summary>Request timeout. Default: 2 minutes</summary>
    public TimeSpan TimeoutMs { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>Additional metadata for the request</summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}
```

### Temperature Guide

| Temperature | Behavior | Best For |
|-------------|----------|----------|
| **0.0** | Deterministic, consistent | Legal docs, compliance |
| **0.1-0.3** | Low randomness, reliable | Business validation |
| **0.4-0.7** | Balanced creativity | General content |
| **0.8-1.0** | More creative | Creative writing validation |
| **1.1+** | High randomness | Experimental use |

### Token Limits by Use Case

| Use Case | Recommended MaxTokens | Rationale |
|----------|----------------------|-----------|
| **Appropriateness check** | 50 | Simple yes/no response |
| **Grammar validation** | 100 | Brief error descriptions |
| **Business emails** | 150 | Detailed feedback |
| **Complex documents** | 300 | Comprehensive analysis |

## 🏠 .NET Aspire Local Setup

### 1. Create Aspire Host Project

```bash
# Create solution
dotnet new sln -n LLMValidatorAspire

# Add Aspire host
dotnet new aspire-apphost -n LLMValidator.AppHost
dotnet sln add LLMValidator.AppHost

# Add your API project
dotnet sln add src/YourApi/YourApi.csproj
```

### 2. Configure AppHost

```csharp
// LLMValidator.AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

// Add Redis for caching (optional)
var redis = builder.AddRedis("cache");

// Add your API with LLMValidator
var api = builder.AddProject<Projects.YourApi>("api")
    .WithReference(redis)
    .WithEnvironment("OpenAI__ApiKey", builder.Configuration["OpenAI:ApiKey"]!);

builder.Build().Run();
```

### 3. Configure API with Aspire

```csharp
// YourApi/Program.cs
builder.AddServiceDefaults(); // Aspire service defaults

// Add LLMValidator with caching
builder.Services.AddLLMValidator();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("cache");
});

// OpenAI client
builder.Services.AddSingleton<IChatClient>(provider =>
    new OpenAIClient(builder.Configuration["OpenAI:ApiKey"])
        .AsChatClient("gpt-4"));

var app = builder.Build();
app.MapDefaultEndpoints(); // Aspire health checks
```

### 4. Run Aspire

```bash
cd LLMValidator.AppHost
dotnet run
```

Access the Aspire dashboard at `https://localhost:17024`

## 💾 Distributed Cache Integration

**Built-in Microsoft.Extensions.AI Caching**

Microsoft.Extensions.AI provides native distributed caching support. LLMValidator automatically benefits from this built-in caching at the IChatClient level - no custom implementation needed!

### Setup with Redis

```csharp
// 1. Add distributed cache service
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "LLMValidator";
});

// 2. Enable caching on your chat client (built into Microsoft.Extensions.AI)
builder.Services.AddSingleton<IChatClient>(provider =>
{
    var client = new OpenAIClient("api-key").AsChatClient("gpt-4");
    return client.UseDistributedCache(); // Microsoft.Extensions.AI built-in caching
});
```

### How It Works

The Microsoft.Extensions.AI caching:
- **Automatic cache key generation** based on complete request (prompt, options, model)
- **Transparent operation** - no changes needed in validation code
- **Identical requests** return cached responses instantly
- **Thread-safe** and **production-ready**

### Benefits

- ✅ **Zero code changes** in validators
- ✅ **Dramatic cost reduction** (avoid duplicate LLM calls)
- ✅ **Faster responses** (~1ms cached vs 500ms+ LLM)
- ✅ **Any IDistributedCache provider** (Redis, SQL Server, Memory)
- ✅ **Automatic cache invalidation** when request parameters change

### Memory Cache Alternative

```csharp
// For development or single-instance scenarios
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IChatClient>(provider =>
{
    var client = new OpenAIClient("api-key").AsChatClient("gpt-4");
    return client.UseDistributedCache(); // Uses memory cache when no distributed cache configured
});
```

### Other Cache Providers

```csharp
// SQL Server distributed cache
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "dbo";
    options.TableName = "CacheEntries";
});

// Azure Service Bus distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("AzureRedis");
});
```

## 🔧 Advanced Extensibility

### Custom Prompt Template Registration

```csharp
public class CustomTamplate : IPromptTemplate 
{
    // Implement all needed propertes here ...
}
```

## 📊 Monitoring and Telemetry

### Built-in Microsoft.Extensions.AI OpenTelemetry

Microsoft.Extensions.AI provides comprehensive built-in OpenTelemetry support for all chat client operations:

```csharp
// Add OpenTelemetry with Microsoft.Extensions.AI instrumentation
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("Microsoft.Extensions.AI")  // Built-in AI instrumentation
        .AddConsoleExporter()
        .AddJaegerExporter());

// Chat client automatically provides telemetry data
builder.Services.AddSingleton<IChatClient>(provider =>
{
    var client = new OpenAIClient("api-key").AsChatClient("gpt-4");
    return client.UseDistributedCache(); // Telemetry works with all middleware
});
```

This configuration guide shows how LLMValidator leverages Microsoft.Extensions.AI to make AI accessible with minimal setup while providing extensive customization options for advanced scenarios.