
![](docs/assets/Dotnet+LLMModels.jpg)

# 📚 LLMValidator Documentation

## 💡 The Vision: AI Made Simple

The creation of **Microsoft.Extensions.AI** enables a bunch of possibilities to integrate AI packages and bring AI to your code. It makes AI accessible for developers without needing to know infrastructure details or complex setup procedures.

**The result?** Developers can use the benefits of AI with just one line of code.

LLMValidator brings the power of Large Language Models (LLMs) to your validation workflows. It **complements** traditional validation by handling scenarios where regex and algorithmic validation fall short - like grammar, tone, meaning, and context.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)

## 📖 Documentation

| Getting Started | Integration | Advanced |
|-----------------|-------------|----------|
| [🎯 Quick Start](https://github.com/gor8808/LLMValidator/blob/master/docs/QuickStart.md) | [🔄 FluentValidation](https://github.com/gor8808/LLMValidator/blob/master/docs/FluentValidation.md) | [🎭 Quality Variants](https://github.com/gor8808/LLMValidator/blob/master/docs/QualityVariants.md) |
| [📝 Prompt Templates](https://github.com/gor8808/LLMValidator/blob/master/docs/PromptTemplates.md) | [⚙️ Configuration](https://github.com/gor8808/LLMValidator/blob/master/docs/Configuration.md) | [🚀 Performance](https://github.com/gor8808/LLMValidator/blob/master/docs/Performance.md) |

LLMValidator is a .NET library that **complements** traditional validation by handling scenarios where regex and algorithmic validation fall short - like grammar, tone, meaning, and context.

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
dotnet add package LLMValidation
dotnet add package LLMValidation.FluentValidation
dotnet add package Microsoft.Extensions.AI.OpenAI
```

## 🚀 Get Started

1. **[Quick Start](https://github.com/gor8808/LLMValidator/blob/master/docs/QuickStart.md)** - 5-minute setup
2. **[Prompt Templates](https://github.com/gor8808/LLMValidator/blob/master/docs/PromptTemplates.md)** - Available validation templates
3. **[FluentValidation](https://github.com/gor8808/LLMValidator/blob/master/docs/FluentValidation.md)** - Integration guide

## 🏆 Features

Built on **Microsoft.Extensions.AI** for maximum compatibility and extensibility. See the [Configuration Guide](https://github.com/gor8808/LLMValidator/blob/master/docs/Configuration.md) for setup details including:

- **Multiple LLM Providers**: OpenAI, Azure, Anthropic, Ollama, local models
- **Distributed Caching**: Redis, SQL Server, Memory caching support
- **Dependency Injection**: Clean resolver pattern, no service locator anti-patterns
- **.NET Aspire**: Full orchestration support with automatic model setup
- **Extensible Architecture**: Custom resolvers, validators, and prompt templates

For complete setup instructions, examples, and advanced configuration, see the **[📖 Documentation](https://github.com/gor8808/LLMValidator/tree/master/docs)** above.
