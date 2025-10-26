using LLMValidation;
using LLMValidation.Example.ConsoleApp.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>()
{
    ["ConnectionStrings:gemma2-connection"] = "Endpoint=http://localhost:12263;Model=gemma2:2b-instruct-q4_0",
    ["ConnectionStrings:llama3-2-connection"] = "Endpoint=http://localhost:12263;Model=llama3.2:3b-instruct-q4_0",
    ["ConnectionStrings:llama3-2-instruct"] = "Endpoint=http://localhost:12263;Model=llama3.2:1b",
    ["ConnectionStrings:tinyllama"] = "Endpoint=http://localhost:12263;Model=tinyllama:1.1b-chat"
}!);

builder.Services.AddDistributedMemoryCache();

// Register multiple Ollama models as keyed chat clients
// The connection names must match what's configured in AppHost
var b = builder.AddOllamaApiClient("llama3-2-connection")
    .AddKeyedChatClient("llama3-2")
    .UseDistributedCache(configure: c =>
    {
        // Include model name into cache key
        c.CacheKeyAdditionalValues = ["llama3-2"];
    });


builder.AddOllamaApiClient("tinyllama")
    .AddKeyedChatClient("tinyllama")
    .UseDistributedCache(configure: c =>
    {
        // Include model name into cache key
        c.CacheKeyAdditionalValues = ["tinyllama"];
    });

builder.AddOllamaApiClient("llama3-2-instruct")
    .AddKeyedChatClient("llama3-2-instruct")
    .UseDistributedCache(configure: c =>
    {
        // Include model name into cache key
        c.CacheKeyAdditionalValues = ["llama3-2-instruct"];
    });

builder.AddOllamaApiClient("gemma2-connection")
    .AddKeyedChatClient("gemma2")
    .UseDistributedCache(configure: c =>
    {
        // Include model name into cache key
        c.CacheKeyAdditionalValues = ["gemma2"];
    });

// Register LLM validator
builder.Services.AddLLMValidator()
    .AddModelOption("llama3-2")
    .AddModelOption("tinyllama")
    .AddModelOption("llama3-2-instruct")
    .AddModelOption("gemma2");

builder.Services.AddSingleton<IBenchmarkService, BenchmarkService>();

var app = builder.Build();

await app.Services.GetRequiredService<IBenchmarkService>().ExecuteAsync(CancellationToken.None);


