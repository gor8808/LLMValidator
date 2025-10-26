using Microsoft.Extensions.DependencyInjection;

namespace LLMValidation;

/// <summary>
/// Builder for configuring LLM validator services.
/// </summary>
public class LLMValidatorBuilder
{
    /// <summary>
    /// The service collection being configured.
    /// </summary>
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

    /// <summary>
    /// Adds default model configuration options.
    /// </summary>
    public LLMValidatorBuilder AddModelOption(Action<LLMValidationModelDefaultOption>? configureAction = null)
    {
        return AddModelOption(LLMValidationModelDefaultOption.DefaultClientName, configureAction);
    }

    /// <summary>
    /// Adds model-specific configuration options.
    /// </summary>
    public LLMValidatorBuilder AddModelOption(string modelName, Action<LLMValidationModelDefaultOption>? configureAction = null)
    {
        Services.AddOptions<LLMValidationModelDefaultOption>(modelName)
            .Configure(configureAction ?? (_ => { }))
            .PostConfigure(m => m.ClientModelName = modelName);

        return this;
    }

    /// <summary>
    /// Adds model-specific configuration options with access to services.
    /// </summary>
    public LLMValidatorBuilder AddModelOption(string modelName, Action<LLMValidationModelDefaultOption, IServiceCollection> configureAction)
    {
        Services.AddOptions<LLMValidationModelDefaultOption>(modelName)
            .Configure(configureAction)
            .PostConfigure(m => m.ClientModelName = modelName);

        return this;
    }

}
