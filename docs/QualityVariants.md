# 🎭 Quality Variants: Speed vs Accuracy Tradeoffs

One of LLMValidator's most powerful features is its **Quality Variant system**. Every validation template comes in three carefully optimized variants, allowing you to balance speed, accuracy, and cost based on your specific needs.

## 🎯 Understanding Quality Variants

### The Three Variants

| Variant | Speed | Accuracy | Cost | Token Usage | Best For |
|---------|-------|----------|------|-------------|----------|
| **🚀 Fast** | ⚡⚡⚡ | ⭐⭐ | 💰 | 20-50 | High-volume, basic validation |
| **⚖️ Balanced** | ⚡⚡ | ⭐⭐⭐ | 💰💰 | 50-150 | General use, default choice |
| **🎯 Accurate** | ⚡ | ⭐⭐⭐⭐⭐ | 💰💰💰 | 150-300 | Critical content, detailed analysis |

### The Philosophy

Quality variants are designed around a fundamental principle: **different content has different validation requirements**. A user comment on a forum needs different validation rigor than a press release or legal document.

```csharp
// User comment - Fast validation
RuleFor(x => x.Comment)
    .MustBeAppropriate(validator, PromptVariant.Fast);

// Blog post - Balanced validation
RuleFor(x => x.Article)
    .MustHaveValidGrammar(validator, PromptVariant.Balanced);

// Press release - Accurate validation
RuleFor(x => x.PressRelease)
    .MustHaveValidGrammar(validator, PromptVariant.Accurate);
```

## 🚀 Fast Variant: High-Volume Processing

### Characteristics
- **Ultra-fast processing** (1-3 seconds typical response time)
- **Minimal token usage** (85%+ cost savings vs Accurate)
- **Basic but reliable** validation
- **Optimized for scale** (handle 100s of validations per minute)

### When to Use Fast
✅ **User-generated content moderation**
✅ **Real-time validation during typing**
✅ **High-volume batch processing**
✅ **Cost-sensitive applications**
✅ **Simple pass/fail decisions**

### Fast Variant Examples

```csharp
// Grammar check - Fast variant
PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Fast);
// Output: "Check grammar and spelling quickly."

// Appropriateness check - Fast variant
PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast);
// Output: "Must be appropriate and safe content."

// Topic validation - Fast variant
PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(PromptVariant.Fast, "technology");
// Output: "Must be about technology."
```

### Real-World Fast Variant Use Case

```csharp
public class CommentModerationService
{
    private readonly ILLMValidator _validator;

    public async Task<ModerationResult> ModerateCommentAsync(string comment)
    {
        // Process 1000+ comments per minute with Fast variant
        var result = await _validator.ValidateAsync(comment, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
            ClientModelName = "gpt-3.5-turbo", // Faster, cheaper model
            TimeoutMs = TimeSpan.FromSeconds(5), // Quick timeout
            MaxTokens = 50 // Minimal response
        });

        return new ModerationResult
        {
            Action = result.IsValid ? ModerationAction.Approve : ModerationAction.Review,
            Confidence = "Basic", // Fast = basic confidence
            ProcessingTime = TimeSpan.FromSeconds(1.2) // Typical fast processing
        };
    }
}
```

### Performance Metrics - Fast Variant
```
Average Response Time: 1-3 seconds (varies by model/provider/network)
Token Usage: 20-50 tokens (depends on prompt complexity and response)
Cost per Validation: Varies by provider (see calculation example below)
Throughput Capacity: Depends on your infrastructure and rate limits

Example Cost Calculation (GPT-4, as of 2024):
- Input: ~30 tokens × $0.03/1K tokens = $0.0009
- Output: ~20 tokens × $0.06/1K tokens = $0.0012
- Total: ~$0.002 per validation (prices change frequently)
```

## ⚖️ Balanced Variant: The Sweet Spot

### Characteristics
- **Good balance** of speed and accuracy
- **Reasonable cost** (3x more than Fast, 3x less than Accurate)
- **Structured validation** with clear criteria
- **Reliable for most use cases**

### When to Use Balanced
- ✅ **Blog posts and articles** (default choice)
- ✅ **Business communications**
- ✅ **Customer support content**
- ✅ **Marketing materials**
- ✅ **Most production scenarios**

### Balanced Variant Examples

```csharp
// Grammar check - Balanced variant
PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced);
/* Output:
Check if the text has correct grammar and spelling.
Look for:
- Spelling errors
- Basic grammar mistakes
- Sentence structure issues
*/

// Email validation - Balanced variant
PromptTemplateExtensions.GetVariant<BusinessValidationPrompts.EmailValidation>(PromptVariant.Balanced);
/* Output:
Check if this is a professional business email.

Requirements:
- Professional tone and language
- Clear subject or purpose
- Appropriate greeting and closing
- Business-appropriate content
*/
```

### Real-World Balanced Variant Use Case

```csharp
public class BlogPostValidationService
{
    public async Task<BlogValidationResult> ValidateBlogPostAsync(BlogPost post)
    {
        var tasks = new List<Task<LLMValidationResult>>();

        // Grammar validation - Balanced for good quality
        tasks.Add(_validator.ValidateAsync(post.Content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced),
            ClientModelName = "gpt-4"
        }));

        // Topic validation - Balanced for good relevance checking
        tasks.Add(_validator.ValidateAsync(post.Content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(
                PromptVariant.Balanced, post.Category),
            ClientModelName = "gpt-4"
        }));

        // Tone validation - Balanced for professional standards
        tasks.Add(_validator.ValidateAsync(post.Content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(
                PromptVariant.Balanced, "professional"),
            ClientModelName = "gpt-4"
        }));

        var results = await Task.WhenAll(tasks);

        return new BlogValidationResult
        {
            IsValid = results.All(r => r.IsValid),
            Issues = results.Where(r => !r.IsValid).Select(r => r.Message).ToList(),
            Confidence = "Good", // Balanced = good confidence
            QualityScore = CalculateQualityScore(results)
        };
    }
}
```

### Performance Metrics - Balanced Variant
```
Average Response Time: 2-5 seconds (varies by model/provider/network)
Token Usage: 50-150 tokens (depends on prompt complexity and response)
Cost per Validation: Varies by provider (see calculation example below)
Throughput Capacity: Depends on your infrastructure and rate limits

Example Cost Calculation (GPT-4, as of 2024):
- Input: ~80 tokens × $0.03/1K tokens = $0.0024
- Output: ~50 tokens × $0.06/1K tokens = $0.003
- Total: ~$0.005 per validation (prices change frequently)
```

## 🎯 Accurate Variant: Maximum Precision

### Characteristics
- **Highest accuracy** and precision
- **Comprehensive analysis** with detailed criteria
- **Context-aware validation**
- **Best for critical content**

### When to Use Accurate
- ✅ **Legal documents and contracts**
- ✅ **Press releases and public statements**
- ✅ **Regulatory compliance content**
- ✅ **High-stakes business communications**
- ✅ **Published articles and documentation**

### Accurate Variant Examples

```csharp
// Grammar check - Accurate variant
PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Accurate);
/* Output:
Thoroughly evaluate the text for grammar and spelling accuracy.

Check for:
- Spelling errors (including homophones)
- Grammar mistakes (subject-verb agreement, tense consistency)
- Punctuation errors
- Sentence structure and clarity
- Word usage and appropriateness

Consider context and intended meaning. Minor stylistic preferences are acceptable.
*/
```

### Real-World Accurate Variant Use Case

```csharp
public class LegalDocumentValidationService
{
    public async Task<LegalValidationResult> ValidateContractAsync(string contractText, ContractType type)
    {
        var validationTasks = new List<Task<LLMValidationResult>>();

        // Grammar validation - Accurate for legal precision
        validationTasks.Add(_validator.ValidateAsync(contractText, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Accurate),
            ClientModelName = "gpt-4",
            Temperature = 0.0f, // Maximum consistency for legal content
            MaxTokens = 300
        }));

        // Tone validation - Accurate for legal appropriateness
        validationTasks.Add(_validator.ValidateAsync(contractText, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<ToneValidationPrompts, string>(
                PromptVariant.Accurate, "formal legal"),
            ClientModelName = "gpt-4",
            Temperature = 0.0f
        }));

        // Content requirements - Accurate for completeness
        var requiredElements = GetRequiredElementsForContract(type);
        validationTasks.Add(_validator.ValidateAsync(contractText, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<ContentRequirementValidationPrompt, string>(
                PromptVariant.Accurate, requiredElements),
            ClientModelName = "gpt-4",
            Temperature = 0.0f
        }));

        var results = await Task.WhenAll(validationTasks);

        return new LegalValidationResult
        {
            IsValid = results.All(r => r.IsValid),
            Issues = results.SelectMany(r => GetDetailedIssues(r)).ToList(),
            Confidence = "High", // Accurate = high confidence
            RequiresLegalReview = !results.All(r => r.IsValid),
            ComplianceScore = CalculateComplianceScore(results)
        };
    }

    private string GetRequiredElementsForContract(ContractType type) => type switch
    {
        ContractType.Employment => "job duties, compensation, termination clauses, confidentiality agreements",
        ContractType.ServiceAgreement => "scope of work, deliverables, payment terms, liability limitations",
        ContractType.NDA => "confidential information definition, permitted uses, breach consequences",
        _ => "parties identification, consideration, terms and conditions, signatures"
    };
}
```

### Performance Metrics - Accurate Variant
```
Average Response Time: 5-15 seconds (varies by model/provider/network)
Token Usage: 150-300 tokens (depends on prompt complexity and response)
Cost per Validation: Varies by provider (see calculation example below)
Throughput Capacity: Depends on your infrastructure and rate limits

Example Cost Calculation (GPT-4, as of 2024):
- Input: ~200 tokens × $0.03/1K tokens = $0.006
- Output: ~100 tokens × $0.06/1K tokens = $0.006
- Total: ~$0.012 per validation (prices change frequently)
```

## 🎨 Smart Variant Selection Strategies

### 1. Content-Based Selection

```csharp
public class SmartValidationService
{
    public PromptVariant SelectVariantForContent(ContentType contentType, int contentLength)
    {
        return contentType switch
        {
            ContentType.SocialMediaPost => PromptVariant.Fast,
            ContentType.Email when contentLength < 500 => PromptVariant.Fast,
            ContentType.Email when contentLength >= 500 => PromptVariant.Balanced,
            ContentType.BlogPost => PromptVariant.Balanced,
            ContentType.Article => PromptVariant.Balanced,
            ContentType.PressRelease => PromptVariant.Accurate,
            ContentType.LegalDocument => PromptVariant.Accurate,
            ContentType.ContractualAgreement => PromptVariant.Accurate,
            _ => PromptVariant.Balanced
        };
    }
}
```

### 2. User-Tier Based Selection

```csharp
public class TieredValidationService
{
    public PromptVariant SelectVariantForUser(UserTier userTier, ContentImportance importance)
    {
        return (userTier, importance) switch
        {
            (UserTier.Free, _) => PromptVariant.Fast,
            (UserTier.Basic, ContentImportance.Low) => PromptVariant.Fast,
            (UserTier.Basic, ContentImportance.Medium) => PromptVariant.Balanced,
            (UserTier.Premium, ContentImportance.Low) => PromptVariant.Balanced,
            (UserTier.Premium, ContentImportance.Medium) => PromptVariant.Balanced,
            (UserTier.Premium, ContentImportance.High) => PromptVariant.Accurate,
            (UserTier.Enterprise, _) => PromptVariant.Accurate,
            _ => PromptVariant.Balanced
        };
    }
}
```

### 3. Volume-Based Selection

```csharp
public class VolumeAwareValidationService
{
    private readonly IMemoryCache _cache;

    public async Task<PromptVariant> SelectVariantBasedOnVolumeAsync()
    {
        var currentLoad = await GetCurrentProcessingLoadAsync();

        return currentLoad switch
        {
            var load when load > 80 => PromptVariant.Fast,    // High load = fast processing
            var load when load > 50 => PromptVariant.Balanced, // Medium load = balanced
            var load when load <= 50 => PromptVariant.Accurate, // Low load = high quality
            _ => PromptVariant.Balanced
        };
    }

    private async Task<int> GetCurrentProcessingLoadAsync()
    {
        // Check current system load, queue depth, etc.
        return await Task.FromResult(65); // Example load percentage
    }
}
```

## 📊 Cost Analysis and Calculation

### How to Calculate Your Actual Costs

**Important**: The costs shown in this documentation are **examples** based on approximate token usage and 2024 pricing. Your actual costs will vary based on:

- **LLM Provider** (OpenAI, Azure, Anthropic, local models)
- **Model Choice** (GPT-4, GPT-3.5-turbo, Claude, etc.)
- **Current Pricing** (prices change frequently)
- **Actual Token Usage** (varies by content length and complexity)
- **Volume Discounts** (many providers offer bulk pricing)

### Cost Calculation Formula

```
Cost per Validation = (Input Tokens × Input Price per Token) + (Output Tokens × Output Price per Token)
```

### Example Calculation (GPT-4, January 2024 pricing):
```
Fast Variant:
- Input: ~30 tokens × $0.03/1K = $0.0009
- Output: ~20 tokens × $0.06/1K = $0.0012
- Total: ~$0.002 per validation

Balanced Variant:
- Input: ~80 tokens × $0.03/1K = $0.0024
- Output: ~50 tokens × $0.06/1K = $0.003
- Total: ~$0.005 per validation

Accurate Variant:
- Input: ~200 tokens × $0.03/1K = $0.006
- Output: ~100 tokens × $0.06/1K = $0.006
- Total: ~$0.012 per validation
```


### Real-World Cost Optimization

Instead of fixed projections, focus on:

1. **Monitor Actual Usage**: Track token consumption in your application
2. **Test Different Providers**: Compare costs across OpenAI, Azure, Anthropic
3. **Optimize Prompts**: Shorter prompts = lower costs
4. **Use Appropriate Variants**: Don't use Accurate when Fast will suffice
5. **Consider Local Models**: For high-volume scenarios, self-hosted models might be cheaper

## 🎯 Variant Selection Decision Tree

```
Start: What type of content are you validating?

├── User-generated content (comments, posts)
│   ├── High volume (>100/min) → Fast
│   └── Medium volume → Balanced
│
├── Business communications
│   ├── Internal email → Fast
│   ├── Customer email → Balanced
│   └── Executive/PR email → Accurate
│
├── Published content
│   ├── Social media → Fast
│   ├── Blog posts → Balanced
│   └── Press releases → Accurate
│
└── Legal/Compliance content
    └── Always → Accurate
```

## 🚀 Advanced Variant Usage Patterns

### 1. Progressive Validation

Start with Fast, escalate to higher variants based on results:

```csharp
public class ProgressiveValidationService
{
    public async Task<ValidationResult> ValidateProgressivelyAsync(string content)
    {
        // Step 1: Fast validation for basic issues
        var fastResult = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
            ClientModelName = "gpt-3.5-turbo"
        });

        // If Fast validation fails, no need to continue
        if (!fastResult.IsValid)
        {
            return ValidationResult.Failed(fastResult.Message, "Basic appropriateness check failed");
        }

        // Step 2: Balanced validation for detailed check
        var balancedResult = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced),
            ClientModelName = "gpt-4"
        });

        // If content is critical, do accurate validation
        if (IsCriticalContent(content))
        {
            var accurateResult = await _validator.ValidateAsync(content, new LLMValidationOptions
            {
                ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Accurate),
                ClientModelName = "gpt-4"
            });

            return ValidationResult.Success(accurateResult, "High-precision validation completed");
        }

        return ValidationResult.Success(balancedResult, "Standard validation completed");
    }
}
```

### 2. Parallel Multi-Variant Validation

Use different variants for different aspects:

```csharp
public class MultiAspectValidationService
{
    public async Task<ComprehensiveValidationResult> ValidateAllAspectsAsync(string content, string category)
    {
        var tasks = new[]
        {
            // Fast appropriateness check - high volume, basic safety
            _validator.ValidateAsync(content, new LLMValidationOptions
            {
                ValidationPrompt = PromptTemplateExtensions.GetVariant<AppropriatenessValidationPrompts>(PromptVariant.Fast),
                ClientModelName = "gpt-3.5-turbo"
            }),

            // Balanced grammar check - good quality/speed balance
            _validator.ValidateAsync(content, new LLMValidationOptions
            {
                ValidationPrompt = PromptTemplateExtensions.GetVariant<GrammarValidationPrompts>(PromptVariant.Balanced),
                ClientModelName = "gpt-4"
            }),

            // Accurate topic validation - critical for categorization
            _validator.ValidateAsync(content, new LLMValidationOptions
            {
                ValidationPrompt = PromptTemplateExtensions.GetVariant<TopicValidationPrompts, string>(
                    PromptVariant.Accurate, category),
                ClientModelName = "gpt-4"
            })
        };

        var results = await Task.WhenAll(tasks);

        return new ComprehensiveValidationResult
        {
            AppropriatenessResult = results[0],
            GrammarResult = results[1],
            TopicResult = results[2],
            IsValid = results.All(r => r.IsValid),
            TotalCost = CalculateTotalCost(results),
            ProcessingTime = CalculateMaxProcessingTime(results)
        };
    }
}
```

### 3. Dynamic Variant Selection

Adjust variants based on real-time conditions:

```csharp
public class DynamicValidationService
{
    public async Task<ValidationResult> ValidateWithDynamicVariantAsync(string content, ValidationContext context)
    {
        var variant = await SelectOptimalVariantAsync(context);
        var model = SelectOptimalModel(variant, context.Budget);

        var result = await _validator.ValidateAsync(content, new LLMValidationOptions
        {
            ValidationPrompt = GetPromptForVariant(context.ValidationType, variant),
            ClientModelName = model,
            Temperature = GetTemperatureForVariant(variant),
            MaxTokens = GetMaxTokensForVariant(variant)
        });

        return new ValidationResult
        {
            Result = result,
            VariantUsed = variant,
            ModelUsed = model,
            OptimizationReason = GetOptimizationReason(variant, context)
        };
    }

    private async Task<PromptVariant> SelectOptimalVariantAsync(ValidationContext context)
    {
        var systemLoad = await GetSystemLoadAsync();
        var queueDepth = await GetQueueDepthAsync();
        var budgetConstraints = context.Budget;

        // Algorithm to select optimal variant based on multiple factors
        if (budgetConstraints == Budget.Low || systemLoad > 90)
            return PromptVariant.Fast;

        if (context.Importance == ValidationImportance.Critical || systemLoad < 30)
            return PromptVariant.Accurate;

        return PromptVariant.Balanced;
    }
}
```

## 🎯 Key Takeaways

### ✅ Do's
- **Match variant to use case** - Critical content gets Accurate, high-volume gets Fast
- **Consider total cost** - Balance accuracy needs with budget constraints
- **Monitor performance** - Track accuracy vs speed tradeoffs
- **Use progressive validation** - Start fast, escalate when needed
- **Combine variants** - Different aspects can use different variants

### ❌ Don'ts
- **Don't always use Accurate** - It's expensive and often unnecessary
- **Don't ignore Fast variant** - It's powerful for high-volume scenarios
- **Don't use Accurate for user comments** - Overkill and cost-prohibitive
- **Don't use Fast for legal content** - Risk of missing critical issues
- **Don't forget about model selection** - Variant choice affects model requirements

## 🚀 Next Steps

Master quality variants for optimal LLM validation:

- **[📝 Prompt Templates](PromptTemplates.md)** - Explore all available templates and variants
- **[⚙️ Configuration](Configuration.md)** - Configure variants for your specific needs
- **[🚀 Performance](Performance.md)** - Optimize variant selection for maximum efficiency

---

**Choose Wisely!** 🎭 Quality variants are your key to balancing speed, accuracy, and cost in LLM validation. The right variant for the right use case makes all the difference!