using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace LLMValidation;

/// <summary>
/// Default implementation that resolves chat clients from dependency injection.
/// </summary>
public class DefaultChatClientResolver : IChatClientResolver
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public DefaultChatClientResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public IChatClient Resolve(string? modelName)
    {
        // If model name is null or empty, try non-keyed registration first
        if (string.IsNullOrEmpty(modelName))
        {
            return _serviceProvider.GetRequiredService<IChatClient>();
        }

        // For named models, use keyed services
        return _serviceProvider.GetRequiredKeyedService<IChatClient>(modelName);
    }
}
