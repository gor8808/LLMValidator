namespace LLMValidation.Example.ConsoleApp.Models;

/// <summary>
/// Represents a test case for LLM validation benchmarking.
/// </summary>
public class TestCase
{
    public required string Name { get; init; }
    public required string Input { get; init; }
    public required string ValidationPrompt { get; init; }
    public required bool ExpectedResult { get; init; }
    public string? Description { get; init; }
}
