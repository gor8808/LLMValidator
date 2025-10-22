using Microsoft.Extensions.DependencyInjection;

namespace LLMValidation;

public class LLMValidatorBuilder
{
    public required IServiceCollection Services { get; init; }

    internal LLMValidatorBuilder AddValidatorCoreServices()
    {
        // Register the default chat client resolver
        Services.AddSingleton<IChatClientResolver, DefaultChatClientResolver>();

        // Register the validator
        Services.AddSingleton<ILLMValidator, LLMValidator>();

        // Add default model option in case only one client is registered
        AddModelOption();

        return this;
    }

    public LLMValidatorBuilder AddModelOption(Action<LLMValidationModelDefaultOption>? configureAction = null)
    {
        return AddModelOption(LLMValidationModelDefaultOption.DefaultClientName, configureAction);
    }

    public LLMValidatorBuilder AddModelOption(string modelName, Action<LLMValidationModelDefaultOption>? configureAction = null)
    {
        Services.AddOptions<LLMValidationModelDefaultOption>(modelName)
            .Configure(configureAction ?? (_ => { }))
            .PostConfigure(m => m.ClientModelName = modelName);

        return this;
    }

    public LLMValidatorBuilder AddModelOption(string modelName, Action<LLMValidationModelDefaultOption, IServiceCollection> configureAction)
    {
        Services.AddOptions<LLMValidationModelDefaultOption>(modelName)
            .Configure(configureAction)
            .PostConfigure(m => m.ClientModelName = modelName);

        return this;
    }

    /// <summary>
    /// Registers a custom chat client resolver.
    /// </summary>
    public LLMValidatorBuilder UseChatClientResolver<TResolver>() where TResolver : class, IChatClientResolver
    {
        Services.AddSingleton<IChatClientResolver, TResolver>();
        return this;
    }

}
