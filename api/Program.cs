using api.Extensions;
using api.Helpers;
using api.Models.Requests;
using api.Models.Responses;
using api.Models.Storage;
using api.Services;
using Azure;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.Net.Http.Headers;
using OpenAI.Embeddings;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

ConfigureKeyvault(builder);
AddServices(builder);
ConfigureCors(builder);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.Run();



static void ConfigureKeyvault(WebApplicationBuilder builder)
{
    var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];

    if (string.IsNullOrEmpty(keyVaultUri))
    {
        Console.WriteLine("KEY_VAULT_URI not found in configuration");
    }
    else
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential(
                        new DefaultAzureCredentialOptions()
                        {
                            ExcludeAzureCliCredential = true,
                            ExcludeAzureDeveloperCliCredential = true,
                            ExcludeAzurePowerShellCredential = true,
                            ExcludeEnvironmentCredential = true,
                            ExcludeInteractiveBrowserCredential = true,
                            ExcludeSharedTokenCacheCredential = true,
                            ExcludeWorkloadIdentityCredential = true,
                        })
        );
    }
}

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    builder.Services.AddAuthorization();

    builder.Services.AddScoped<IDbService, DbService>();

    string apiKey = builder.Configuration["OPENAI_API_KEY"] ??
                    Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                    ?? throw new Exception("OpenAI API key is missing");
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
    
    //HuggingFace Model:
    //builder.Services.AddHttpClient("E5LMLApi", options =>
    //{
    //    options.BaseAddress = new Uri("http://localhost:8000/");
    //    options.DefaultRequestHeaders.Add("Accept", "application/json");
    //});
    //builder.Services.AddScoped<IE5LMLService, E5LMLService>(sp =>
    //{
    //    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    //    var client = httpClientFactory.CreateClient("E5LMLApi");
    //    return new E5LMLService(client);
    //});
}

static void ConfigureCors(WebApplicationBuilder builder)
{
    var allowedOrigins = (builder.Configuration["ALLOWED_ORIGINS"] ?? "")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization)
                .AllowAnyMethod();
        });
    });
}