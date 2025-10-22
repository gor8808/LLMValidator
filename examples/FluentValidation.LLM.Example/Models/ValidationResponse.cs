namespace FluentValidation.LLM.Example.Models;

/// <summary>
/// Response model for validation results.
/// </summary>
public class ValidationResponse
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public List<ValidationError>? Errors { get; set; }
}
