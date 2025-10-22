using LLMValidation.Example.ConsoleApp.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LLMValidation.Example.ConsoleApp.Services;

/// <summary>
/// Background service that runs LLM validation benchmarks against multiple models.
/// </summary>
public class BenchmarkService : IBenchmarkService
{
    private readonly ILogger<BenchmarkService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public BenchmarkService(
        ILogger<BenchmarkService> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting LLM Validation Benchmark...");

            // Get all registered model names
            var modelNames = new[]
            {
                "gemma2",
                "llama3-2",
                "tinyllama",
                "llama3-2-instruct"
            };

            var results = new List<ModelBenchmarkResult>();

            // Run benchmarks for each model
            foreach (var modelName in modelNames)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                _logger.LogInformation("Benchmarking model: {ModelName}", modelName);

                var result = await BenchmarkModelAsync(modelName, stoppingToken);
                results.Add(result);
            }

            // Display results
            DisplayResults(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during benchmarking");
        }
        finally
        {
            // Stop the application when done
            _applicationLifetime.StopApplication();
        }
    }

    private async Task<ModelBenchmarkResult> BenchmarkModelAsync(
        string modelName,
        CancellationToken cancellationToken)
    {
        var result = new ModelBenchmarkResult
        {
            ModelName = modelName,
            TotalTests = 0
        };

        var validator = _serviceProvider.GetRequiredService<ILLMValidator>();

        var testCases = TestCases.GetAllTests();

        foreach (var testCase in testCases)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            result.TotalTests++;

            try
            {
                var stopwatch = Stopwatch.StartNew();

                var validationResult = await validator.ValidateAsync(
                    testCase.Input,
                    new LLMValidationOptions
                    {
                        ValidationPrompt = testCase.ValidationPrompt,
                        ClientModelName = modelName // Use the specific model
                    },
                    cancellationToken);

                stopwatch.Stop();
                result.ResponseTimes.Add(stopwatch.Elapsed);

                // Check if the result matches expected
                if (validationResult.IsValid == testCase.ExpectedResult)
                {
                    result.CorrectPredictions++;
                    _logger.LogDebug("✓ {TestName}: PASS ({Duration}ms)",
                        testCase.Name,
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    result.IncorrectPredictions++;
                    _logger.LogWarning("✗ {TestName}: FAIL - Expected {Expected}, Got {Actual} ({Duration}ms)",
                        testCase.Name,
                        testCase.ExpectedResult,
                        validationResult.IsValid,
                        stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                result.IncorrectPredictions++;
                result.Errors.Add($"{testCase.Name}: {ex.Message}");
                _logger.LogError(ex, "Error testing {TestName}", testCase.Name);
            }
        }

        return result;
    }

    private void DisplayResults(List<ModelBenchmarkResult> results)
    {
        // Sort by accuracy (descending), then by average response time (ascending)
        var sortedResults = results
            .OrderByDescending(r => r.AccuracyPercentage)
            .ThenBy(r => r.AverageResponseTime)
            .ToList();

        var resultString = string.Empty;
        foreach (var result in sortedResults)
        {
            resultString += $"""
                               Model: {result.ModelName}
                               Accuracy:           {result.AccuracyPercentage:F2}% ({result.CorrectPredictions}/{result.TotalTests} correct)
                               Avg Response Time:  {result.AverageResponseTime.TotalMilliseconds:F0}ms
                               Min Response Time:  {result.MinResponseTime.TotalMilliseconds:F0}ms
                               result.MaxResponseTime.TotalMilliseconds:  {result.MaxResponseTime.TotalMilliseconds:F0}ms
                               Errors: {result.Errors.Count}"
                               
                               --
                               """;
        }
        
        _logger.LogInformation(resultString);
    }
}
