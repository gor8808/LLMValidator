using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LLMValidation.Tests.Resolvers;

public class DefaultChatClientResolverTests
{
    private readonly IFixture _fixture;

    public DefaultChatClientResolverTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Resolve_WithNullModelName_ShouldResolveNonKeyedClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockChatClient = new Mock<IChatClient>();
        services.AddSingleton(mockChatClient.Object);

        var serviceProvider = services.BuildServiceProvider();
        var resolver = new DefaultChatClientResolver(serviceProvider);

        // Act
        var result = resolver.Resolve(null);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mockChatClient.Object);
    }

    [Fact]
    public void Resolve_WithEmptyModelName_ShouldResolveNonKeyedClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockChatClient = new Mock<IChatClient>();
        services.AddSingleton(mockChatClient.Object);

        var serviceProvider = services.BuildServiceProvider();
        var resolver = new DefaultChatClientResolver(serviceProvider);

        // Act
        var result = resolver.Resolve(string.Empty);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mockChatClient.Object);
    }

    [Theory]
    [InlineData("gpt-4")]
    [InlineData("claude")]
    [InlineData("llama")]
    public void Resolve_WithModelName_ShouldResolveKeyedClient(string modelName)
    {
        // Arrange
        var services = new ServiceCollection();
        var mockChatClient = new Mock<IChatClient>();
        services.AddKeyedSingleton(modelName, mockChatClient.Object);

        var serviceProvider = services.BuildServiceProvider();
        var resolver = new DefaultChatClientResolver(serviceProvider);

        // Act
        var result = resolver.Resolve(modelName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(mockChatClient.Object);
    }

    [Fact]
    public void Resolve_WithNullModelName_WhenNoClientRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var resolver = new DefaultChatClientResolver(serviceProvider);

        // Act
        var act = () => resolver.Resolve(null);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*IChatClient*");
    }

    [Fact]
    public void Resolve_WithModelName_WhenKeyedClientNotRegistered_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var resolver = new DefaultChatClientResolver(serviceProvider);

        // Act
        var act = () => resolver.Resolve("non-existent-model");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*non-existent-model*");
    }

    [Fact]
    public void Resolve_WithMultipleKeyedClients_ShouldResolveCorrectOne()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockChatClient1 = new Mock<IChatClient>();
        var mockChatClient2 = new Mock<IChatClient>();
        services.AddKeyedSingleton("model1", mockChatClient1.Object);
        services.AddKeyedSingleton("model2", mockChatClient2.Object);

        var serviceProvider = services.BuildServiceProvider();
        var resolver = new DefaultChatClientResolver(serviceProvider);

        // Act
        var result1 = resolver.Resolve("model1");
        var result2 = resolver.Resolve("model2");

        // Assert
        result1.Should().BeSameAs(mockChatClient1.Object);
        result2.Should().BeSameAs(mockChatClient2.Object);
        result1.Should().NotBeSameAs(result2);
    }
}
