using Microsoft.Extensions.DependencyInjection;

namespace LLMValidation;

/// <summary>
/// Extension methods for registering LLM validation services.
/// </summary>
public static class LLMValidatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers the LLM validator service with the specified chat client from DI.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static LLMValidatorBuilder AddLLMValidator(this IServiceCollection services)
    {
        return new LLMValidatorBuilder { Services = services }.AddValidatorCoreServices();
    }
}
