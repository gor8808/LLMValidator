namespace LLMValidation.Example.ConsoleApp.Models;

/// <summary>
/// Predefined test cases for benchmarking LLM validators.
/// </summary>
public static class TestCases
{
    public static readonly List<TestCase> GrammarTests = new()
    {
        new TestCase
        {
            Name = "Valid Grammar",
            Input = "The quick brown fox jumps over the lazy dog.",
            ValidationPrompt = "Check if the following text has correct grammar and spelling.",
            ExpectedResult = true,
            Description = "Correct grammar and spelling"
        },
        new TestCase
        {
            Name = "Grammar Error - Subject-Verb Agreement",
            Input = "The dogs is running in the park.",
            ValidationPrompt = "Check if the following text has correct grammar and spelling.",
            ExpectedResult = false,
            Description = "Subject-verb agreement error"
        },
        new TestCase
        {
            Name = "Spelling Error",
            Input = "I recieved your messege yesterday.",
            ValidationPrompt = "Check if the following text has correct grammar and spelling.",
            ExpectedResult = false,
            Description = "Multiple spelling errors"
        },
        new TestCase
        {
            Name = "Valid Complex Sentence",
            Input = "Although she was tired, she continued working on the project because the deadline was approaching.",
            ValidationPrompt = "Check if the following text has correct grammar and spelling.",
            ExpectedResult = true,
            Description = "Complex sentence with correct grammar"
        }
    };

    public static readonly List<TestCase> TopicTests = new()
    {
        new TestCase
        {
            Name = "About Dogs - Valid",
            Input = "Golden Retrievers are friendly and loyal dogs that make great family pets.",
            ValidationPrompt = "Check if the following text is about dogs.",
            ExpectedResult = true,
            Description = "Text clearly about dogs"
        },
        new TestCase
        {
            Name = "About Cats - Invalid",
            Input = "Persian cats are known for their long, fluffy coats and calm temperament.",
            ValidationPrompt = "Check if the following text is about dogs.",
            ExpectedResult = false,
            Description = "Text about cats, not dogs"
        },
        new TestCase
        {
            Name = "About Technology - Invalid",
            Input = "The new smartphone features a powerful processor and high-resolution camera.",
            ValidationPrompt = "Check if the following text is about dogs.",
            ExpectedResult = false,
            Description = "Text about technology, not dogs"
        },
        new TestCase
        {
            Name = "About Dog Breeds - Valid",
            Input = "The German Shepherd is an intelligent and versatile working dog breed.",
            ValidationPrompt = "Check if the following text is about dogs.",
            ExpectedResult = true,
            Description = "Text about specific dog breed"
        }
    };

    public static readonly List<TestCase> ContentTests = new()
    {
        new TestCase
        {
            Name = "Contains Dog Terminology - Valid",
            Input = "The Labrador puppy was playing fetch in the backyard.",
            ValidationPrompt = "Check if the following text contains dog breed names or dog-related terminology.",
            ExpectedResult = true,
            Description = "Contains 'Labrador' and 'puppy'"
        },
        new TestCase
        {
            Name = "No Dog Terminology - Invalid",
            Input = "The animal was running around the yard.",
            ValidationPrompt = "Check if the following text contains dog breed names or dog-related terminology.",
            ExpectedResult = false,
            Description = "Generic animal reference, no dog-specific terms"
        },
        new TestCase
        {
            Name = "Contains Breed Name - Valid",
            Input = "I saw a beautiful Siberian Husky at the park today.",
            ValidationPrompt = "Check if the following text contains dog breed names or dog-related terminology.",
            ExpectedResult = true,
            Description = "Contains specific breed name"
        }
    };

    public static List<TestCase> GetAllTests()
    {
        return GrammarTests
            .Concat(TopicTests)
            .Concat(ContentTests)
            .ToList();
    }
}
