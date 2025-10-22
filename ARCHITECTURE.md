# FluentValidation.LLM - Architecture

## Design Principles

1. **Lightweight** - Minimal dependencies, no caching infrastructure, simple codebase
2. **Provider Agnostic** - Built on Microsoft.Extensions.AI abstractions
3. **Structured Outputs Only** - Uses `ChatResponseFormat.ForJsonSchema` to guarantee JSON responses
4. **Type Safe** - Strongly typed validation responses
5. **Integration First** - Seamless FluentValidation integration

## Core Components

### 1. LLMValidator
**Purpose**: Main validation engine that calls the LLM and parses responses

**Key Features**:
- Simple constructor accepting `IChatClient`
- Async validation with cancellation support
- Guaranteed JSON output via `ForJsonSchema`
- Timeout handling (default: 30 seconds)
- ConfigureAwait(false) for better async performance

**Code Size**: ~110 lines

### 2. LLMValidationOptions
**Purpose**: Configuration for each validation request

**Properties**:
- `ValidationPrompt` (required): What to validate
- `SystemPrompt` (optional): Override default system instructions
- `ErrorMessage` (optional): Custom error message on failure
- `MaxTokens` (optional): Response size limit (default: 200)
- `Temperature` (optional): Randomness control (default: 0.3)
- `TimeoutMs`: Request timeout (default: 30000)
- `Metadata`: Additional provider-specific options

### 3. LLMValidationResponse
**Purpose**: Structured JSON schema for LLM responses

```json
{
  "is_valid": true/false,
  "reason": "explanation here"
}
```

### 4. LLMValidatorExtensions
**Purpose**: FluentValidation helper methods

**Pre-built Validators**:
- `MustPassLLMValidation` - Generic validation with full control
- `MustHaveValidGrammar` - Grammar and spelling check
- `MustBeAbout` - Topic validation
- `MustContain` - Content requirement validation
- `MustHaveTone` - Tone/sentiment validation
- `MustBeAppropriate` - Content safety check

### 5. ServiceCollectionExtensions
**Purpose**: Dependency injection registration

**Methods**:
- `AddLLMValidator()` - Register with IChatClient from DI
- `AddLLMValidator(factory)` - Register with custom factory
- `AddLLMValidator(chatClient)` - Register with specific instance

## Why No Caching?

**Reasons**:
1. **Keep it Simple** - Caching adds complexity and state management
2. **Validation Changes** - Input data changes frequently in validation scenarios
3. **User Control** - Users can implement their own caching strategy if needed
4. **Memory Footprint** - No memory overhead for cache storage
5. **Deterministic** - Each validation call is independent and predictable

## Why Structured Outputs Only?

**Benefits**:
1. **Guaranteed Format** - `ForJsonSchema` ensures valid JSON responses
2. **No String Parsing** - Direct deserialization, no regex or contains() checks
3. **Type Safety** - Compile-time validation of response structure
4. **Better Performance** - No fallback parsing logic needed
5. **AI-Friendly** - Modern LLMs handle structured outputs natively

## Token Optimization

**System Prompt** (concise):
```
Validate text and respond ONLY with JSON: {"is_valid":true/false,"reason":"explanation"}
```

**Why Concise?**:
- Costs less per request
- Faster response times
- Clear instructions
- No unnecessary tokens

## Performance Considerations

### What We Do
- ✅ Use `ConfigureAwait(false)` for async calls
- ✅ Reuse static `JsonSerializerOptions`
- ✅ Pre-allocate list capacity where known
- ✅ Default reasonable token limits (200)
- ✅ Timeout protection

### What Users Should Do
- Run validations in parallel for independent fields
- Use appropriate temperature (lower = more deterministic)
- Set MaxTokens based on validation complexity
- Consider implementing application-level caching if needed
- Use local models (Ollama) for lower latency

## Integration Flow

```
User Code
   └─> FluentValidation Validator
         └─> MustPassLLMValidation()
               └─> ILLMValidator.ValidateAsync()
                     └─> IChatClient.CompleteAsync()
                           └─> LLM Provider (OpenAI/Ollama/etc)
                                 └─> JSON Response
                                       └─> LLMValidationResult
```

## Provider Examples

### Ollama (Local)
```csharp
services.AddChatClient(builder =>
    builder.UseOllama("llama3.2", new Uri("http://localhost:11434")));
```

### OpenAI
```csharp
services.AddChatClient(builder =>
    builder.Use(new OpenAIClient(apiKey).AsChatClient("gpt-4o-mini")));
```

### Azure OpenAI
```csharp
services.AddChatClient(builder =>
    builder.UseAzureOpenAI(endpoint, credential, deploymentName));
```

## File Structure

```
src/FluentValidation.LLM/
├── ILLMValidator.cs                    # Interface
├── LLMValidator.cs                     # Implementation (~110 lines)
├── LLMValidationOptions.cs             # Configuration
├── LLMValidationResult.cs              # Result wrapper
├── LLMValidationResponse.cs            # JSON schema
├── LLMValidatorExtensions.cs           # FluentValidation helpers
└── ServiceCollectionExtensions.cs      # DI registration
```

## Key Metrics

- **Total Lines of Code**: ~500
- **Dependencies**: 3 (FluentValidation, Microsoft.Extensions.AI.Abstractions, Microsoft.Extensions.DependencyInjection.Abstractions)
- **Classes**: 7
- **Public API Surface**: Small and focused
- **Null Safety**: Enabled
- **Async First**: All I/O operations are async

## Future Considerations

### If Users Need Caching
Implement at application level:
```csharp
public class CachedLLMValidator : ILLMValidator
{
    private readonly ILLMValidator _inner;
    private readonly IMemoryCache _cache;
    // ... implementation
}
```

### If Users Need Retry Logic
Use Polly or similar:
```csharp
services.AddLLMValidator(sp =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    // Wrap chatClient with retry decorator
    return new LLMValidator(retryWrappedClient);
});
```

### If Users Need Rate Limiting
Implement at IChatClient level or use middleware

## Design Trade-offs

| Decision | Pro | Con |
|----------|-----|-----|
| No caching | Simple, lightweight | More LLM calls |
| Structured outputs only | Reliable, type-safe | Less flexible prompting |
| Async only | Modern, scalable | No sync API |
| Singleton lifetime | Efficient | Less isolation |
| Default timeouts | Safe defaults | May need tuning |

## Summary

FluentValidation.LLM is designed to be a **thin, focused layer** between FluentValidation and any LLM provider. It handles the essential concerns (prompt building, JSON parsing, error handling) while leaving advanced features (caching, retry, rate limiting) to the user or higher-level libraries.

The result is a **simple, maintainable, and efficient** package that does one thing well: LLM-based string validation.
