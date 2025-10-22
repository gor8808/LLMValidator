using Microsoft.Extensions.AI;

namespace LLMValidation;

/// <summary>
/// Resolves IChatClient instances from the service provider.
/// Implement this to customize how chat clients are resolved for different model names.
/// </summary>
public interface IChatClientResolver
{
    /// <summary>
    /// Resolves a chat client for the specified model name.
    /// </summary>
    /// <param name="modelName">The model name/key. If null or empty, resolves the default client.</param>
    /// <returns>The resolved chat client.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no suitable client can be found.</exception>
    IChatClient Resolve(string? modelName);
}
