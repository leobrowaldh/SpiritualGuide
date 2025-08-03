using Azure.Identity;
using FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Embeddings;

var builder = FunctionsApplication.CreateBuilder(args);

#region "Env variable Secret Access(consumption plan dont allow keyvault)"

var openApiKey = builder.Configuration["OPENAI_API_KEY"]
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new Exception("OpenAI API key is missing");
#endregion

#region "Keyvault Secret Access"
//var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];

//if (string.IsNullOrEmpty(keyVaultUri))
//{
//    Console.WriteLine("KEY_VAULT_URI not found in configuration");
//}
//else
//{
//    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential(
//                    new DefaultAzureCredentialOptions()
//                    {
//                        ExcludeAzureCliCredential = true,
//                        ExcludeAzureDeveloperCliCredential = true,
//                        ExcludeAzurePowerShellCredential = true,
//                        ExcludeEnvironmentCredential = true,
//                        ExcludeInteractiveBrowserCredential = true,
//                        ExcludeSharedTokenCacheCredential = true,
//                        ExcludeWorkloadIdentityCredential = true,
//                    })
//    );
//}
#endregion

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

string apiKey = builder.Configuration["OPENAI_API_KEY"] ??
                Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new Exception("OpenAI API key is missing");

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddSingleton(sp => new EmbeddingClient("text-embedding-3-small", apiKey));
builder.Services.AddScoped<IOpenAiService, OpenAiService>();
builder.Services.AddAzureClients(clientBuilder =>
{
    //Managed identity when deployed:
    var uriString = builder.Configuration["AzureWebJobsStorage:tableServiceUri"]
        ?? builder.Configuration["AzureWebJobsStorage__tableServiceUri"]
        ?? Environment.GetEnvironmentVariable("AzureWebJobsStorage__tableServiceUri");

    if (string.IsNullOrWhiteSpace(uriString))
    {
        throw new InvalidOperationException(
            "Missing configuration: 'AzureWebJobsStorage__tableServiceUri'. This must be set in the environment variables or App Settings."
        );
    }

    clientBuilder
        .AddTableServiceClient(new Uri(uriString))
        .WithName("QuotesTableClient");

    //for local dev:
    /*clientBuilder.AddTableServiceClient(builder.Configuration["AzureWebJobsStorage"]).WithName("QuotesTableClient")*/
    ;
});

builder.Services.AddHttpClient("E5LMLApi", options =>
{
    options.BaseAddress = new Uri("http://localhost:8000/");
    options.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddScoped<IE5LMLService, E5LMLService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient("E5LMLApi");
    return new E5LMLService(client);
});

builder.Build().Run();
