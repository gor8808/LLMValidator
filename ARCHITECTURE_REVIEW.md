# Architecture Review & Recommendations

## Current Issues

### 1. **String-Based Branching (Primary Issue)**
```csharp
// ChatClientExtensions.cs - Lines 10-12, 24-26
if (clientName == LLMValidationModelDefaultOption.DefaultClientName)
{
    // Special case for default
}
```
**Problems:**
- Fragile: Magic string comparisons
- Not extensible: Adding new client types requires modifying core code
- Violates Open/Closed Principle

### 2. **Service Locator Anti-Pattern**
```csharp
// LLMValidator.cs - Line 20
public LLMValidator(IServiceProvider serviceProvider)
```
**Problems:**
- Hides dependencies
- Makes testing harder
- Violates Dependency Inversion Principle
- Runtime errors instead of compile-time errors

### 3. **Mixed Responsibilities**
`LLMValidator` does too much:
- Resolves chat clients
- Resolves and merges options
- Performs validation
- Parses responses

### 4. **Keyed Services Dependency**
```csharp
// Forces all integrations to use keyed services
var defaultOptions = _serviceProvider.GetRequiredKeyedService<IOptions<...>>(options.ClientModelName).Value;
```
**Problems:**
- Assumes keyed services are always used
- Default case requires special handling
- Not compatible with simple DI scenarios

### 5. **Validation at Registration**
```csharp
.Validate(_ => Services.HasChatClient(modelName), ...)
.ValidateOnStart();
```
**Problems:**
- Scans entire service collection (expensive)
- Fragile string matching
- Fails at startup even if client is registered later

---

## Recommended Architecture

### Strategy 1: **Factory Pattern with Strategy for Client Resolution**

#### New Structure:
```
LLMValidation/
├── Core/
│   ├── ILLMValidator.cs
│   ├── LLMValidator.cs (simplified)
│   ├── LLMValidationOptions.cs
│   └── LLMValidationResult.cs
├── Configuration/
│   ├── ILLMValidatorFactory.cs (NEW)
│   ├── LLMValidatorFactory.cs (NEW)
│   ├── IChatClientResolver.cs (NEW)
│   ├── DefaultChatClientResolver.cs (NEW)
│   ├── NamedChatClientResolver.cs (NEW)
│   └── LLMValidationOptionsProvider.cs (NEW)
└── DependencyInjection/
    ├── LLMValidatorBuilder.cs (refactored)
    └── ServiceCollectionExtensions.cs (refactored)
```

---

## Detailed Refactoring Plan

### Phase 1: Introduce Abstractions

#### 1. Create `IChatClientResolver`
```csharp
namespace LLMValidation.Configuration;

/// <summary>
/// Strategy for resolving IChatClient instances.
/// </summary>
public interface IChatClientResolver
{
    /// <summary>
    /// Resolves a chat client for the given model name.
    /// </summary>
    IChatClient Resolve(IServiceProvider serviceProvider, string? modelName = null);

    /// <summary>
    /// Checks if this resolver can handle the given model name.
    /// </summary>
    bool CanResolve(string? modelName);
}
```

#### 2. Implement Concrete Resolvers
```csharp
// Default resolver (non-keyed)
public class DefaultChatClientResolver : IChatClientResolver
{
    public IChatClient Resolve(IServiceProvider serviceProvider, string? modelName = null)
        => serviceProvider.GetRequiredService<IChatClient>();

    public bool CanResolve(string? modelName)
        => string.IsNullOrEmpty(modelName) || modelName == "default";
}

// Named/Keyed resolver
public class KeyedChatClientResolver : IChatClientResolver
{
    public IChatClient Resolve(IServiceProvider serviceProvider, string? modelName = null)
    {
        if (string.IsNullOrEmpty(modelName))
            throw new ArgumentNullException(nameof(modelName));

        return serviceProvider.GetRequiredKeyedService<IChatClient>(modelName);
    }

    public bool CanResolve(string? modelName)
        => !string.IsNullOrEmpty(modelName);
}

// Composite resolver (chain of responsibility)
public class CompositeChatClientResolver : IChatClientResolver
{
    private readonly IEnumerable<IChatClientResolver> _resolvers;

    public CompositeChatClientResolver(IEnumerable<IChatClientResolver> resolvers)
        => _resolvers = resolvers;

    public IChatClient Resolve(IServiceProvider serviceProvider, string? modelName = null)
    {
        var resolver = _resolvers.FirstOrDefault(r => r.CanResolve(modelName))
            ?? throw new InvalidOperationException($"No resolver found for model '{modelName}'");

        return resolver.Resolve(serviceProvider, modelName);
    }

    public bool CanResolve(string? modelName)
        => _resolvers.Any(r => r.CanResolve(modelName));
}
```

#### 3. Create Factory
```csharp
namespace LLMValidation.Configuration;

public interface ILLMValidatorFactory
{
    ILLMValidator Create(string? modelName = null);
}

public class LLMValidatorFactory : ILLMValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IChatClientResolver _clientResolver;
    private readonly IOptionsMonitor<LLMValidationModelDefaultOption> _optionsMonitor;

    public LLMValidatorFactory(
        IServiceProvider serviceProvider,
        IChatClientResolver clientResolver,
        IOptionsMonitor<LLMValidationModelDefaultOption> optionsMonitor)
    {
        _serviceProvider = serviceProvider;
        _clientResolver = clientResolver;
        _optionsMonitor = optionsMonitor;
    }

    public ILLMValidator Create(string? modelName = null)
    {
        var chatClient = _clientResolver.Resolve(_serviceProvider, modelName);
        var options = _optionsMonitor.Get(modelName ?? Options.DefaultName);

        return new LLMValidator(chatClient, options);
    }
}
```

### Phase 2: Simplify LLMValidator

#### Refactored Validator (NO Service Locator)
```csharp
public class LLMValidator : ILLMValidator
{
    private readonly IChatClient _chatClient;
    private readonly LLMValidationModelDefaultOption _defaultOptions;

    // Clean constructor - all dependencies explicit
    public LLMValidator(
        IChatClient chatClient,
        LLMValidationModelDefaultOption defaultOptions)
    {
        _chatClient = chatClient;
        _defaultOptions = defaultOptions;
    }

    public async Task<LLMValidationResult> ValidateAsync(
        string value,
        LLMValidationOptions options,
        CancellationToken cancellationToken = default)
    {
        // Merge options
        var effectiveOptions = MergeOptions(options, _defaultOptions);

        // Build messages
        var messages = BuildMessages(effectiveOptions, value);

        // Configure chat
        var chatOptions = BuildChatOptions(effectiveOptions);

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(effectiveOptions.TimeoutMs);

            var response = await _chatClient.GetResponseAsync(messages, chatOptions, cts.Token);

            return ParseJsonResponse(response.Text, effectiveOptions);
        }
        catch (OperationCanceledException)
        {
            return LLMValidationResult.Failure("Validation timed out.");
        }
        catch (Exception ex)
        {
            return LLMValidationResult.Failure($"Validation error: {ex.Message}");
        }
    }

    // Private helper methods...
}
```

### Phase 3: Improve Registration API

#### Better Builder Pattern
```csharp
public static class LLMValidatorServiceCollectionExtensions
{
    public static IServiceCollection AddLLMValidation(
        this IServiceCollection services,
        Action<LLMValidatorBuilder>? configure = null)
    {
        // Register core services
        services.AddSingleton<ILLMValidatorFactory, LLMValidatorFactory>();

        // Register default resolvers
        services.AddSingleton<IChatClientResolver, DefaultChatClientResolver>();
        services.AddSingleton<IChatClientResolver, KeyedChatClientResolver>();
        services.AddSingleton<IChatClientResolver, CompositeChatClientResolver>(sp =>
            new CompositeChatClientResolver(sp.GetServices<IChatClientResolver>()
                .Where(r => r is not CompositeChatClientResolver)));

        // Register validator using factory
        services.AddTransient<ILLMValidator>(sp =>
            sp.GetRequiredService<ILLMValidatorFactory>().Create());

        // Configure
        var builder = new LLMValidatorBuilder(services);
        configure?.Invoke(builder);

        return services;
    }
}

public class LLMValidatorBuilder
{
    private readonly IServiceCollection _services;

    public LLMValidatorBuilder(IServiceCollection services)
        => _services = services;

    /// <summary>
    /// Configure default options for all models.
    /// </summary>
    public LLMValidatorBuilder ConfigureDefaults(Action<LLMValidationModelDefaultOption> configure)
    {
        _services.Configure(configure);
        return this;
    }

    /// <summary>
    /// Configure options for a specific model.
    /// </summary>
    public LLMValidatorBuilder ConfigureModel(string modelName, Action<LLMValidationModelDefaultOption> configure)
    {
        _services.Configure(modelName, configure);
        return this;
    }

    /// <summary>
    /// Add a custom client resolver.
    /// </summary>
    public LLMValidatorBuilder AddClientResolver<TResolver>() where TResolver : class, IChatClientResolver
    {
        _services.AddSingleton<IChatClientResolver, TResolver>();
        return this;
    }

    /// <summary>
    /// Replace the default factory.
    /// </summary>
    public LLMValidatorBuilder UseFactory<TFactory>() where TFactory : class, ILLMValidatorFactory
    {
        _services.Replace(ServiceDescriptor.Singleton<ILLMValidatorFactory, TFactory>());
        return this;
    }
}
```

---

## Usage Examples

### Example 1: Simple Usage (Default Client)
```csharp
builder.Services.AddChatClient(/* default client */);

builder.Services.AddLLMValidation(llm => llm
    .ConfigureDefaults(opt =>
    {
        opt.MaxTokens = 100;
        opt.Temperature = 0.3f;
    }));
```

### Example 2: Multiple Named Clients
```csharp
builder.Services.AddKeyedChatClient("gpt-4", /* ... */);
builder.Services.AddKeyedChatClient("claude", /* ... */);

builder.Services.AddLLMValidation(llm =>
{
    llm.ConfigureModel("gpt-4", opt => opt.Temperature = 0.2f);
    llm.ConfigureModel("claude", opt => opt.Temperature = 0.5f);
});

// Usage
var factory = services.GetRequiredService<ILLMValidatorFactory>();
var gptValidator = factory.Create("gpt-4");
var claudeValidator = factory.Create("claude");
```

### Example 3: Custom Resolver
```csharp
public class EnvironmentBasedResolver : IChatClientResolver
{
    public IChatClient Resolve(IServiceProvider sp, string? modelName)
    {
        var env = sp.GetRequiredService<IHostEnvironment>();
        var key = env.IsDevelopment() ? "dev-model" : "prod-model";
        return sp.GetRequiredKeyedService<IChatClient>(key);
    }

    public bool CanResolve(string? modelName) => modelName == "auto";
}

builder.Services.AddLLMValidation(llm => llm
    .AddClientResolver<EnvironmentBasedResolver>());
```

---

## Benefits of This Approach

### 1. **No More Branching**
- Strategy pattern handles different resolution scenarios
- Chain of responsibility for fallback behavior
- Open for extension, closed for modification

### 2. **Explicit Dependencies**
```csharp
// Before: Hidden dependencies
public LLMValidator(IServiceProvider serviceProvider)

// After: Clear dependencies
public LLMValidator(IChatClient chatClient, LLMValidationModelDefaultOption options)
```

### 3. **Testability**
```csharp
// Easy to test with mocks
var mockClient = new Mock<IChatClient>();
var options = new LLMValidationModelDefaultOption();
var validator = new LLMValidator(mockClient.Object, options);
```

### 4. **Flexibility**
- Clients can provide custom resolvers
- No assumptions about keyed services
- Works with any DI configuration

### 5. **Standards Compliance**
- Follows Microsoft.Extensions.* patterns
- Similar to HttpClientFactory
- Familiar to .NET developers

### 6. **Performance**
- No service collection scanning
- No runtime string comparisons
- Options resolved once, not per validation

---

## Migration Path

### Step 1: Add new interfaces alongside existing code
### Step 2: Implement new resolvers
### Step 3: Update LLMValidator to accept IChatClient directly
### Step 4: Update registration methods
### Step 5: Mark old methods as [Obsolete]
### Step 6: Remove deprecated code in next major version

---

## Additional Recommendations

### 1. **Response Parser Strategy**
Allow custom parsers for different response formats:
```csharp
public interface ILLMResponseParser
{
    LLMValidationResult Parse(string response, LLMValidationOptions options);
}
```

### 2. **Validation Pipeline**
Use middleware pattern for extensibility:
```csharp
public interface ILLMValidationMiddleware
{
    Task<LLMValidationResult> InvokeAsync(
        LLMValidationContext context,
        Func<Task<LLMValidationResult>> next);
}
```

### 3. **Typed Options**
Use strongly-typed options per model:
```csharp
services.AddLLMValidation()
    .AddModel<Gpt4Options>("gpt-4")
    .AddModel<ClaudeOptions>("claude");
```

### 4. **Health Checks**
Integrate with .NET health checks:
```csharp
services.AddLLMValidation()
    .AddHealthChecks();
```

---

## Summary

The main architectural improvements are:

1. **Remove string-based branching** → Use Strategy pattern with `IChatClientResolver`
2. **Remove service locator** → Use Factory pattern with explicit dependencies
3. **Separate concerns** → Split resolver, validator, parser into distinct classes
4. **Improve extensibility** → Allow custom resolvers, parsers, middleware
5. **Follow .NET conventions** → Similar to HttpClientFactory pattern

This creates a more maintainable, testable, and extensible architecture that follows SOLID principles and .NET best practices.
