using Azure.Identity;
using FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Embeddings;

var builder = FunctionsApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];

if (string.IsNullOrEmpty(keyVaultUri))
{
    Console.WriteLine("KEY_VAULT_URI not found in configuration");
}
//TODO: optimize DefaultAzureCredential to improve performance and avoid unnecessary authentication attempts
else { builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential()); }

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Read API key from configuration or environment
string apiKey = builder.Configuration["open-ai-key"] ??
                Environment.GetEnvironmentVariable("open-ai-key")
                ?? throw new Exception("OpenAI API key is missing");

builder.Services.AddSingleton(sp => new EmbeddingClient("text-embedding-3-small", apiKey));
builder.Services.AddScoped<IAiService, AiService>();

builder.Build().Run();
