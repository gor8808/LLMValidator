namespace FluentValidation.LLM.Example.Models;

/// <summary>
/// Represents a single validation error.
/// </summary>
public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
