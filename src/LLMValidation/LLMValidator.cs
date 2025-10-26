using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LLMValidation;

/// <summary>
/// LLM-based validation using Microsoft.Extensions.AI with structured JSON outputs.
/// </summary>
public class LLMValidator : ILLMValidator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IChatClientResolver _chatClientResolver;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Creates LLMValidator instance.
    /// </summary>
    public LLMValidator(
        IServiceProvider serviceProvider,
        IChatClientResolver chatClientResolver)
    {
        _serviceProvider = serviceProvider;
        _chatClientResolver = chatClientResolver;
    }

    /// <summary>
    /// Validates text using LLM with specified options.
    /// </summary>
    /// <param name="value">Text to validate</param>
    /// <param name="options">Validation configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with success/failure status</returns>
    public async Task<LLMValidationResult> ValidateAsync(
        string value,
        LLMValidationOptions options,
        CancellationToken cancellationToken = default)
    {
        // Resolve chat client using the resolver
        var chatClient = _chatClientResolver.Resolve(options.ClientModelName);

        // Get default options for this model
        var defaultOptions = _serviceProvider.GetRequiredService<IOptionsMonitor<LLMValidationModelDefaultOption>>().Get(options.ClientModelName);

        // Merge options with defaults
        var effectiveOptions = options.WithDefaults(defaultOptions);

        // Build messages
        var messages = BuildMessages(effectiveOptions, defaultOptions, value);

        // Configure chat options
        var chatOptions = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema<LLMValidationResponse>(JsonOptions),
            MaxOutputTokens = effectiveOptions.MaxTokens,
            Temperature = effectiveOptions.Temperature
        };

        if (effectiveOptions.Metadata.Count > 0)
        {
            chatOptions.AdditionalProperties = new AdditionalPropertiesDictionary(effectiveOptions.Metadata);
        }

        try
        {
            // Invoke LLM with timeout
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(effectiveOptions.TimeoutMs);

            var response = await chatClient.GetResponseAsync(messages, chatOptions, cts.Token);

            // Parse response
            return ParseJsonResponse(response.Text, effectiveOptions);
        }
        catch (OperationCanceledException)
        {
            return LLMValidationResult.Failure("Validation timed out.");
        }
    }

    private static List<ChatMessage> BuildMessages(
        LLMValidationOptions options,
        LLMValidationModelDefaultOption defaultOptions,
        string value)
    {
        var messages = new List<ChatMessage>();

        // Add system prompt from defaults if provided
        if (defaultOptions.SystemPrompt != null)
        {
            messages.Add(new ChatMessage(ChatRole.System, defaultOptions.SystemPrompt));
        }

        // Add system prompt from options if provided (overrides default)
        if (options.SystemPrompt != null)
        {
            messages.Add(new ChatMessage(ChatRole.System, options.SystemPrompt));
        }

        // Add validation prompt and value
        messages.Add(new ChatMessage(ChatRole.User, options.ValidationPrompt));
        messages.Add(new ChatMessage(ChatRole.User, $"Text to validate: {value}"));

        return messages;
    }

    private static LLMValidationResult ParseJsonResponse(string responseText, LLMValidationOptions options)
    {
        var validationResponse = JsonSerializer.Deserialize<LLMValidationResponse>(responseText, JsonOptions);

        if (validationResponse == null)
        {
            throw new JsonException("Unable to parse LLM response");
        }

        // Check confidence threshold if specified
        if (options.MinConfidence.HasValue && validationResponse.Confidence.HasValue)
        {
            if (validationResponse.Confidence.Value < options.MinConfidence.Value)
            {
                var confidenceMessage = $"Validation confidence ({validationResponse.Confidence.Value:F2}) is below minimum threshold ({options.MinConfidence.Value:F2}).";
                return LLMValidationResult.Failure(confidenceMessage, responseText);
            }
        }

        if (validationResponse.IsValid)
        {
            return LLMValidationResult.Success(validationResponse.Reason, responseText);
        }
        else
        {
            var errorMessage = options.ErrorMessage ?? validationResponse.Reason ?? "Validation failed.";
            return LLMValidationResult.Failure(errorMessage, responseText);
        }
    }
}
