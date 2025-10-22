namespace LLMValidation.Example.ConsoleApp.Services;
public interface IBenchmarkService
{
    public Task ExecuteAsync(CancellationToken stoppingToken);
}
