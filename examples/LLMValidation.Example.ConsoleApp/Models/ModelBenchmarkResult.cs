namespace LLMValidation.Example.ConsoleApp.Models;

/// <summary>
/// Contains benchmark results for a specific model.
/// </summary>
public class ModelBenchmarkResult
{
    public required string ModelName { get; init; }
    public int TotalTests { get; set; }
    public int CorrectPredictions { get; set; }
    public int IncorrectPredictions { get; set; }
    public List<TimeSpan> ResponseTimes { get; } = new();
    public List<string> Errors { get; } = new();

    public double AccuracyPercentage => TotalTests > 0
        ? (CorrectPredictions / (double)TotalTests) * 100
        : 0;

    public TimeSpan AverageResponseTime => ResponseTimes.Count > 0
        ? TimeSpan.FromMilliseconds(ResponseTimes.Average(t => t.TotalMilliseconds))
        : TimeSpan.Zero;

    public TimeSpan MinResponseTime => ResponseTimes.Count > 0
        ? ResponseTimes.Min()
        : TimeSpan.Zero;

    public TimeSpan MaxResponseTime => ResponseTimes.Count > 0
        ? ResponseTimes.Max()
        : TimeSpan.Zero;
}
