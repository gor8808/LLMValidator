
![](docs/assets/Dotnet+LLMModels.jpg)

# 📚 LLMValidator Documentation

Welcome to the **LLMValidator** documentation! This library provides powerful LLM-based validation for .NET applications using Microsoft.Extensions.AI.

## 📖 Documentation

| Getting Started | Integration | Advanced |
|-----------------|-------------|----------|
| [🎯 Quick Start](QuickStart.md) | [🔄 FluentValidation](FluentValidation.md) | [🎭 Quality Variants](QualityVariants.md) |
| [📝 Prompt Templates](PromptTemplates.md) | [⚙️ Configuration](Configuration.md) | [🚀 Performance](Performance.md) |

## 🎯 What is LLMValidator?

LLMValidator is a .NET library that brings the power of Large Language Models (LLMs) to your validation workflows. It **complements** traditional validation by handling scenarios where regex and algorithmic validation fall short - like grammar, tone, meaning, and context.

```csharp
// Traditional validation - Perfect for format validation
public bool IsValidEmailFormat(string email)
{
    var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    return Regex.IsMatch(email, pattern); // Fast, reliable, offline
}

// LLM validation - For things traditional validation can't handle
var result = await validator.ValidateAsync(emailContent, new LLMValidationOptions
{
    ValidationPrompt = "Check if this email is professional and follows business communication standards",
    ClientModelName = "gpt-4"
});

// Best practice: Combine both approaches
public async Task<bool> ValidateBusinessEmailAsync(string email, string content)
{
    // 1. Format validation first (fast, reliable)
    if (!IsValidEmailFormat(email))
        return false;

    // 2. Content validation with LLM (for nuanced checks)
    var contentResult = await validator.ValidateAsync(content, new LLMValidationOptions
    {
        ValidationPrompt = "Check if this email content is professional and appropriate",
        ClientModelName = "gpt-4"
    });

    return contentResult.IsValid;
}
```

## ✨ Key Features

- **🧠 Natural Language Rules**: Describe validation in plain English
- **⚡ Quality Variants**: Fast/Balanced/Accurate modes for different needs
- **🔄 FluentValidation Integration**: Works with existing validation
- **📝 Built-in Templates**: Grammar, tone, appropriateness, business content
- **🎯 Multiple Models**: OpenAI, Azure, Anthropic, local models

## 🌟 Why LLMValidator?

**Traditional validation** (regex, algorithms) is perfect for format/structure validation.
**LLM validation** handles what traditional validation can't: grammar, tone, meaning, context.

Use **both together** for comprehensive validation.

## 📦 Installation

```bash
# Core library
dotnet add package LLMValidation

# FluentValidation integration
dotnet add package LLMValidation.FluentValidation
```

## 🏃‍♂️ 30-Second Quick Start

```csharp
// 1. Register services
builder.Services.AddLLMValidator()
    .AddModelOption("gpt-4", opt => opt.Temperature = 0.1f);

// 2. Register your LLM client
builder.Services.AddSingleton<IChatClient>(provider =>
    new YourLLMClient("gpt-4"));

// 3. Use in your code
public class MyController : ControllerBase
{
    private readonly ILLMValidator _validator;

    public MyController(ILLMValidator validator) => _validator = validator;

    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostRequest request)
    {
        var result = await _validator.ValidateAsync(request.Content, new LLMValidationOptions
        {
            ValidationPrompt = "Check if this content is appropriate for a professional blog",
            ClientModelName = "gpt-4"
        });

        if (!result.IsValid)
            return BadRequest(result.Message);

        // Continue with post creation...
        return Ok();
    }
}
```

## 🚀 Get Started

1. **[Quick Start](QuickStart.md)** - 5-minute setup
2. **[Prompt Templates](PromptTemplates.md)** - Available validation templates
3. **[FluentValidation](FluentValidation.md)** - Integration guide