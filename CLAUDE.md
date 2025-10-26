# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LLMValidator is a .NET 9.0 library that enables semantic validation using Large Language Models through Microsoft.Extensions.AI. The creation of Microsoft.Extensions.AI makes AI accessible for developers without needing infrastructure knowledge - enabling AI benefits with just one line of code.

### Core Philosophy: Complementary, Not Replacement

**CRITICAL**: LLMValidator **complements** traditional validation, it does NOT replace it. Traditional validation (regex, algorithms) works perfectly for format/structure validation and should continue to be used. LLM validation is only used where traditional validation cannot work - for grammar, tone, meaning, context, and semantic understanding.

### Performance-First Design

- Memory/performance optimized to not cause issues on production systems
- Built-in Microsoft.Extensions.AI caching reduces costs and improves response times
- Quality variants (Fast/Balanced/Accurate) allow speed vs accuracy tradeoffs
- Designed for API endpoint validations with minimal overhead

## Architecture

### Core Components

- **LLMValidation** - Main library with core validation engine (`src/LLMValidation/`)
- **LLMValidation.FluentValidation** - FluentValidation integration extensions (`src/LLMValidation.FluentValidation/`)
- **Examples** - Comprehensive examples including API, console app, and Aspire orchestration (`examples/`)
- **Tests** - Unit tests using xUnit, AutoFixture, AutoMoq, Moq, and FluentAssertions (`tests/`)

### Key Interfaces and Classes

- `ILLMValidator` - Main validation interface
- `LLMValidator` - Core implementation using Microsoft.Extensions.AI
- `IChatClientResolver` - Resolves chat clients by model name (supports keyed services)
- `LLMValidationOptions` - Configuration class with immutable defaults merging
- `LLMValidatorExtensions` - FluentValidation helper methods

### Prompt Template System

- `IPromptTemplate` - Interface for structured prompt templates
- `PromptVariant` enum - Fast/Balanced/Accurate quality variants
- `ValidationPrompts` - Comprehensive collection of well-tested prompt templates
- **Direct Access**: Use `BusinessValidationPrompts.ProposalValidation.Fast` when variant is known at compile time
- **Dynamic Access**: Use extension methods for runtime variant selection



### Dependency Injection Pattern

Uses clean resolver pattern instead of service locator:
- `IChatClientResolver` handles model-specific client resolution
- Supports both default and keyed IChatClient services
- Immutable options merging with `WithDefaults()` method

## Common Development Commands

### Build and Test
```bash
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Run tests in specific project
dotnet test tests/LLMValidation.Tests/

# Build release packages
dotnet build -c Release
```

### Running Examples
```bash
# Run Aspire orchestration (starts everything)
cd examples/LLMValidation.Example.AppHost
dotnet run

# Run API example directly
cd examples/LLMValidation.Example.Api
dotnet run

# Run console benchmarking app
cd examples/LLMValidation.Example.ConsoleApp
dotnet run
```

## Project Structure

### Solution Organization
- `src/` - Core library projects
- `examples/` - Sample applications (API, console, Aspire)
- `tests/` - Unit and integration tests

### Key Files
- `LLMValidator.cs:11` - Main validator implementation
- `LLMValidationOptions.cs:53` - Options merging logic with `WithDefaults()`
- `LLMValidatorExtensions.cs` - FluentValidation helper methods
- `IChatClientResolver.cs` - Client resolution interface
- `ValidationPrompts.cs` - Restructured prompt template system with IPromptTemplate interfaces

### Documentation Structure
- `docs/` - Comprehensive documentation following FusionCache style
- `docs/Configuration.md` - Microsoft.Extensions.AI setup, DI extensibility, Aspire integration
- `docs/QuickStart.md` - 5-minute setup guide
- `docs/AGentleIntroduction.md` - Philosophy emphasizing complementary approach
- `docs/PromptTemplates.md` - Template system usage (direct vs dynamic access)
- `docs/QualityVariants.md` - Speed vs accuracy tradeoffs with cost considerations

## Development Guidelines

### Testing Framework
- Uses xUnit as test framework
- AutoFixture for test data generation
- AutoMoq for automatic mocking
- FluentAssertions for readable assertions
- Tests focus on immutability, options merging, and resolver patterns

### Microsoft.Extensions.AI Integration
- Built on IChatClient abstraction for universal AI provider support
- Supports structured JSON responses using ChatResponseFormat.ForJsonSchema<LLMValidationResponse>()
- **Built-in Features**: Distributed caching, OpenTelemetry monitoring, request/response logging
- Compatible with any LLM provider (OpenAI, Azure, Anthropic, Ollama, local models)
- **Caching**: Uses Microsoft.Extensions.AI built-in caching (not custom implementations)
- **Telemetry**: Leverages Microsoft.Extensions.AI OpenTelemetry integration (not custom wrappers)

### Options Pattern
- LLMValidationOptions are immutable after merging
- WithDefaults() creates new instances, never mutates originals
- Supports model-specific default configurations
- Metadata is merged from defaults and user options

### FluentValidation Integration
- Extension methods follow FluentValidation patterns
- Async validation with MustAsync
- Proper error message handling
- Supports method chaining with IRuleBuilderOptions

## Package Information

Two NuGet packages:
- **LLMValidation** - Core validation engine
- **LLMValidation.FluentValidation** - FluentValidation extensions

Both target .NET 9.0 with nullable reference types enabled.

## Project Management

### Central Package Management
- Uses Directory.Build.props (root) and Directory.Packages.props for centralized package versioning
- Separate src/Directory.Build.props for packable projects under `/src` folder
- All dependencies managed centrally for consistency

### Documentation Philosophy
- **Concise and Practical**: Documentation is streamlined for easy integration
- **FusionCache Style**: Clear navigation, practical examples, minimal text
- **Complementary Emphasis**: Always emphasize that LLM validation complements (never replaces) traditional validation
- **Microsoft.Extensions.AI First**: Document built-in capabilities before custom implementations

## Important Development Notes

### What NOT to Do
- **Never position LLM validation as replacement for traditional validation**
- **Never create custom caching/telemetry when Microsoft.Extensions.AI has built-in support**
- **Never make documentation overly verbose** - keep it concise and practical
- **Never include placeholder cost calculations** - use proper methodology or disclaimers

### Best Practices
- Emphasize complementary approach in all documentation
- Use Microsoft.Extensions.AI built-in features first
- Keep documentation concise but comprehensive
- Provide both direct template access and dynamic runtime selection examples
- Include proper cost calculation methodology with disclaimers about variable costs