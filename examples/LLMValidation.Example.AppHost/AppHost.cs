var builder = DistributedApplication.CreateBuilder(args);

// Add the Ollama container
var ollama = builder
    .AddOllama("ollama")          // docker.io/ollama/ollama
    .WithDataVolume()             // persist downloaded models across runs
    .WithOpenWebUI();             // optional: adds Open WebUI pointing to Ollama

// Pre-pull multiple models for benchmarking
var llama32 = ollama.AddModel("llama3-2-connection", "llama3.2:3b-instruct-q4_0"); // ok
var gemma2 = ollama.AddModel("gemma2-connection", "gemma2:2b-instruct-q4_0"); // k
var llama1b = ollama.AddModel("llama3-2-instruct", "llama3.2:1b"); // ~1B (good quality for its size)
var tiny11b = ollama.AddModel("tinyllama", "tinyllama:1.1b-chat");  // ~1.1B (very fast)

// API service using default model (for backward compatibility)
var ollamaModel = ollama.AddModel("ollama-model", "llama3.2:3b-instruct-fp16");
var api = builder
    .AddProject<Projects.LLMValidation_Example_Api>("example-api")
    .WithReference(ollamaModel)
    .WaitFor(ollamaModel);

// Console benchmarking app using all models
var benchmarkApp = builder
    .AddProject<Projects.LLMValidation_Example_ConsoleApp>("benchmark-console")
    .WithReference(llama32)
    .WithReference(llama1b)
    .WithReference(tiny11b)
    .WithReference(gemma2)
    .WaitFor(llama32)
    .WaitFor(llama1b)
    .WaitFor(tiny11b)
    .WaitFor(gemma2);

builder.Build().Run();
