using AutoFixture;
using FluentAssertions;
using Xunit;

namespace LLMValidation.Tests;

public class LLMValidationOptionsTests
{
    [Fact]
    public void WithDefaults_ShouldReturnNewInstance()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model"
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            ClientModelName = "default-model",
            MaxTokens = 100,
            Temperature = 0.5f
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.Should().NotBeSameAs(options, "it should return a new instance");
    }

    [Fact]
    public void WithDefaults_WhenMaxTokensIsNull_ShouldUseDefault()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            MaxTokens = null
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            MaxTokens = 200
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.MaxTokens.Should().Be(200);
    }

    [Fact]
    public void WithDefaults_WhenMaxTokensIsSet_ShouldKeepOriginalValue()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            MaxTokens = 500
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            MaxTokens = 200
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.MaxTokens.Should().Be(500);
    }

    [Fact]
    public void WithDefaults_WhenTemperatureIsNull_ShouldUseDefault()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            Temperature = null
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            Temperature = 0.7f
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.Temperature.Should().Be(0.7f);
    }

    [Fact]
    public void WithDefaults_WhenTemperatureIsSet_ShouldKeepOriginalValue()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            Temperature = 0.2f
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            Temperature = 0.7f
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.Temperature.Should().Be(0.2f);
    }

    [Fact]
    public void WithDefaults_WhenSystemPromptIsNull_ShouldUseDefault()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            SystemPrompt = null
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            SystemPrompt = "Default system prompt"
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.SystemPrompt.Should().Be("Default system prompt");
    }

    [Fact]
    public void WithDefaults_WhenSystemPromptIsSet_ShouldKeepOriginalValue()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            SystemPrompt = "Custom system prompt"
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            SystemPrompt = "Default system prompt"
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.SystemPrompt.Should().Be("Custom system prompt");
    }

    [Fact]
    public void WithDefaults_WhenTimeoutIsZero_ShouldUseDefault()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            TimeoutMs = TimeSpan.Zero
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            TimeoutMs = TimeSpan.FromSeconds(30)
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.TimeoutMs.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void WithDefaults_WhenTimeoutIsSet_ShouldKeepOriginalValue()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            TimeoutMs = TimeSpan.FromSeconds(60)
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            TimeoutMs = TimeSpan.FromSeconds(30)
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.TimeoutMs.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void WithDefaults_ShouldMergeMetadata()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            Metadata = new Dictionary<string, object?>
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            }
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            Metadata = new Dictionary<string, object?>
            {
                ["key2"] = "default2", // Should not override
                ["key3"] = "value3"    // Should be added
            }
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.Metadata.Should().HaveCount(3);
        result.Metadata["key1"].Should().Be("value1");
        result.Metadata["key2"].Should().Be("value2", "original values should not be overridden");
        result.Metadata["key3"].Should().Be("value3");
    }

    [Fact]
    public void WithDefaults_ShouldNotMutateOriginalOptions()
    {
        // Arrange
        var originalMetadata = new Dictionary<string, object?> { ["key1"] = "value1" };
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test prompt",
            ClientModelName = "test-model",
            MaxTokens = null,
            Metadata = originalMetadata
        };

        var defaults = new LLMValidationModelDefaultOption
        {
            MaxTokens = 200,
            Metadata = new Dictionary<string, object?> { ["key2"] = "value2" }
        };

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        options.MaxTokens.Should().BeNull("original should not be mutated");
        options.Metadata.Should().HaveCount(1, "original metadata should not be mutated");
        options.Metadata.Should().NotContainKey("key2");
    }

    [Fact]
    public void WithDefaults_ShouldPreserveAllRequiredProperties()
    {
        // Arrange
        var options = new LLMValidationOptions
        {
            ValidationPrompt = "Test validation prompt",
            ClientModelName = "test-model",
            ErrorMessage = "Custom error"
        };

        var defaults = new LLMValidationModelDefaultOption();

        // Act
        var result = options.WithDefaults(defaults);

        // Assert
        result.ValidationPrompt.Should().Be("Test validation prompt");
        result.ClientModelName.Should().Be("test-model");
        result.ErrorMessage.Should().Be("Custom error");
    }
}
