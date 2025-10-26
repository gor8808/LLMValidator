using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using LLMValidation.FluentValidation;
using Moq;
using Xunit;

namespace LLMValidation.Tests.FluentValidation;

public class LLMValidatorExtensionsTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ILLMValidator> _mockLLMValidator;

    public LLMValidatorExtensionsTests()
    {
        _fixture = new Fixture();
        _mockLLMValidator = new Mock<ILLMValidator>();
    }

    [Fact]
    public async Task MustPassLLMValidation_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LLMValidationResult.Success());

        var validator = new TestValidator(_mockLLMValidator.Object);
        var model = new TestModel { Description = "Valid description" };

        // Act
        var result = await validator.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task MustPassLLMValidation_WhenInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var errorMessage = "Invalid content";
        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LLMValidationResult.Failure(errorMessage));

        var validator = new TestValidator(_mockLLMValidator.Object);
        var model = new TestModel { Description = "Invalid description" };

        // Act
        var result = await validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(errorMessage);
    }

    [Fact]
    public async Task MustPassLLMValidation_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new TestValidator(_mockLLMValidator.Object);
        var model = new TestModel { Description = "" };

        // Act
        var result = await validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
        _mockLLMValidator.Verify(v => v.ValidateAsync(
            It.IsAny<string>(),
            It.IsAny<LLMValidationOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MustPassLLMValidation_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new TestValidator(_mockLLMValidator.Object);
        var model = new TestModel { Description = null };

        // Act
        var result = await validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
        _mockLLMValidator.Verify(v => v.ValidateAsync(
            It.IsAny<string>(),
            It.IsAny<LLMValidationOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MustPassLLMValidation_WithCustomErrorMessage_ShouldUseCustomMessage()
    {
        // Arrange
        var customError = "Custom error message";
        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(LLMValidationResult.Failure("LLM error"));

        var validator = new TestValidatorWithCustomError(_mockLLMValidator.Object, customError);
        var model = new TestModel { Description = "test" };

        // Act
        var result = await validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage(customError);
    }

    [Fact]
    public async Task MustHaveValidGrammar_ShouldCallValidatorWithCorrectPrompt()
    {
        // Arrange
        LLMValidationOptions? capturedOptions = null;
        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, LLMValidationOptions, CancellationToken>((val, opts, ct) =>
            {
                capturedOptions = opts;
            })
            .ReturnsAsync(LLMValidationResult.Success());

        var validator = new GrammarValidator(_mockLLMValidator.Object);
        var model = new TestModel { Description = "Test text" };

        // Act
        await validator.TestValidateAsync(model);

        // Assert
        capturedOptions.Should().NotBeNull();
        capturedOptions!.ValidationPrompt.Should().Contain("grammar");
        capturedOptions.ValidationPrompt.Should().Contain("spelling");
    }

    [Fact]
    public async Task MustBeAbout_ShouldCallValidatorWithCorrectTopic()
    {
        // Arrange
        var topic = "dogs";
        LLMValidationOptions? capturedOptions = null;

        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, LLMValidationOptions, CancellationToken>((val, opts, ct) =>
            {
                capturedOptions = opts;
            })
            .ReturnsAsync(LLMValidationResult.Success());

        var validator = new TopicValidator(_mockLLMValidator.Object, topic);
        var model = new TestModel { Description = "Dogs are great" };

        // Act
        await validator.TestValidateAsync(model);

        // Assert
        capturedOptions.Should().NotBeNull();
        capturedOptions!.ValidationPrompt.Should().Contain(topic);
    }

    [Fact]
    public async Task MustContain_ShouldCallValidatorWithCorrectContent()
    {
        // Arrange
        var requiredContent = "pricing information";
        LLMValidationOptions? capturedOptions = null;

        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, LLMValidationOptions, CancellationToken>((val, opts, ct) =>
            {
                capturedOptions = opts;
            })
            .ReturnsAsync(LLMValidationResult.Success());

        var validator = new ContentValidator(_mockLLMValidator.Object, requiredContent);
        var model = new TestModel { Description = "Price is $99" };

        // Act
        await validator.TestValidateAsync(model);

        // Assert
        capturedOptions.Should().NotBeNull();
        capturedOptions!.ValidationPrompt.Should().Contain(requiredContent);
    }

    [Fact]
    public async Task Extensions_WithClientModelName_ShouldPassCorrectModelName()
    {
        // Arrange
        var modelName = "gpt-4";
        LLMValidationOptions? capturedOptions = null;

        _mockLLMValidator.Setup(v => v.ValidateAsync(
                It.IsAny<string>(),
                It.IsAny<LLMValidationOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, LLMValidationOptions, CancellationToken>((val, opts, ct) =>
            {
                capturedOptions = opts;
            })
            .ReturnsAsync(LLMValidationResult.Success());

        var validator = new ModelNameValidator(_mockLLMValidator.Object, modelName);
        var model = new TestModel { Description = "Test" };

        // Act
        await validator.TestValidateAsync(model);

        // Assert
        capturedOptions.Should().NotBeNull();
        capturedOptions!.ClientModelName.Should().Be(modelName);
    }

    // Test Models and Validators

    private class TestModel
    {
        public string? Description { get; set; }
    }

    private class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator(ILLMValidator llmValidator)
        {
            RuleFor(x => x.Description)
                .MustPassLLMValidation(llmValidator, "Test validation");
        }
    }

    private class TestValidatorWithCustomError : AbstractValidator<TestModel>
    {
        public TestValidatorWithCustomError(ILLMValidator llmValidator, string errorMessage)
        {
            RuleFor(x => x.Description)
                .MustPassLLMValidation(llmValidator, new LLMValidationOptions
                {
                    ClientModelName = LLMValidationModelDefaultOption.DefaultClientName,
                    ValidationPrompt = "Test",
                    ErrorMessage = errorMessage
                });
        }
    }

    private class GrammarValidator : AbstractValidator<TestModel>
    {
        public GrammarValidator(ILLMValidator llmValidator)
        {
            RuleFor(x => x.Description)
                .MustHaveValidGrammar(llmValidator);
        }
    }

    private class TopicValidator : AbstractValidator<TestModel>
    {
        public TopicValidator(ILLMValidator llmValidator, string topic)
        {
            RuleFor(x => x.Description)
                .MustBeAbout(llmValidator, topic);
        }
    }

    private class ContentValidator : AbstractValidator<TestModel>
    {
        public ContentValidator(ILLMValidator llmValidator, string requiredContent)
        {
            RuleFor(x => x.Description)
                .MustContainContent(llmValidator, requiredContent);
        }
    }

    private class ModelNameValidator : AbstractValidator<TestModel>
    {
        public ModelNameValidator(ILLMValidator llmValidator, string modelName)
        {
            RuleFor(x => x.Description)
                .MustHaveValidGrammar(llmValidator, modelName);
        }
    }
}
