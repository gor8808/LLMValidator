using FluentValidation;
using FluentValidation.AspNetCore;
using LLMValidation;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Register Ollama chat client using Aspire
builder.AddOllamaApiClient("ollama-model")
    .AddChatClient();

// Register LLM validator
builder.Services.AddLLMValidator();

// Add FluentValidation with automatic validation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// Register all validators in the assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "LLM Validation API",
        Version = "v1",
        Description = "API demonstrating LLM-based validation using FluentValidation"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LLM Validation API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
