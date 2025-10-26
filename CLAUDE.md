# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LLMValidator is a .NET 9.0 library that enables semantic validation using Large Language Models through Microsoft.Extensions.AI. It provides fluent validation extensions and supports any LLM provider.

This package is deisnged to help developers to easy integrate with any LLM solution to write API endpoint validations quickly. The package is designed with memory/performence first approach to not cause issues on user's machine.

Other then providing the package the solution comes with ready-to-use prompts that are Well tested for LLM Validation

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
- `ValidationPrompts.cs` - Predefined validation prompts

## Development Guidelines

### Testing Framework
- Uses xUnit as test framework
- AutoFixture for test data generation
- AutoMoq for automatic mocking
- FluentAssertions for readable assertions
- Tests focus on immutability, options merging, and resolver patterns

### Microsoft.Extensions.AI Integration
- Built on IChatClient abstraction
- Supports structured JSON responses
- Uses ChatResponseFormat.ForJsonSchema<LLMValidationResponse>()
- Compatible with any LLM provider (OpenAI, Ollama, Azure, etc.)

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