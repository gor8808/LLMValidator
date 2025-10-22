using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace LLMValidation;

public class DefaultChatClientResolver : IChatClientResolver
{
    private readonly IServiceProvider _serviceProvider;
    public DefaultChatClientResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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
