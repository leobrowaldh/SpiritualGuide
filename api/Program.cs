using api.Extensions;
using api.Services;
using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using OpenAI.Embeddings;

var builder = WebApplication.CreateBuilder(args);
var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
}).CreateLogger("Startup");

logger.LogInformation("🔍 Starting up...");

ConfigureSecrets(builder, logger);
AddServices(builder);
ConfigureCors(builder);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapEndpoints();

app.Run();



static void ConfigureSecrets(WebApplicationBuilder builder, ILogger logger)
{
    var keyVaultUri = builder.Configuration["KEY_VAULT_URI"];

    if (builder.Environment.IsDevelopment())
    {
        builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);
        logger.LogInformation("✅ Using local secrets.json for development");

        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            logger.LogInformation("🔍 Attempting to connect to Key Vault (dev) at: {keyVaultUri}", keyVaultUri);
            try
            {
                builder.Configuration.AddAzureKeyVault(
                    new Uri(keyVaultUri),
                    new DefaultAzureCredential()
                );
                logger.LogInformation("✅ Connected to Azure Key Vault using DefaultAzureCredential.");
            }
            catch (Exception ex)
            {
                logger.LogError("❌ Failed to connect to Azure Key Vault (dev): {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }
    }
    else
    {
        if (string.IsNullOrEmpty(keyVaultUri))
        {
            logger.LogError("❌ KEY_VAULT_URI not found in configuration");
            throw new InvalidOperationException("KEY_VAULT_URI is required in production.");
        }

        logger.LogInformation("🔍 Attempting to connect to Key Vault (prod) at: {keyVaultUri}", keyVaultUri);
        try
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new ManagedIdentityCredential()
            );
            logger.LogInformation("✅ Connected to Azure Key Vault using ManagedIdentityCredential.");
        }
        catch (Exception ex)
        {
            logger.LogError("❌ Failed to connect to Azure Key Vault (prod): {ExceptionType} - {Message}", ex.GetType().Name, ex.Message);
            throw;
        }
    }
}



static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("AccessAsUser", policy =>
            policy.RequireClaim("http://schemas.microsoft.com/identity/claims/scope", "access_as_user"));

    builder.Services.AddScoped<IDbService, DbService>();

    string apiKey = builder.Configuration["open-ai-key"] ??
                    Environment.GetEnvironmentVariable("open-ai-key")
                    ?? throw new Exception("OpenAI API key is missing");
    builder.Services.AddSingleton(sp => new EmbeddingClient("text-embedding-3-small", apiKey));

    builder.Services.AddScoped<IOpenAiService, OpenAiService>();

    var uriString = builder.Configuration["AzureWebJobsStorage:tableServiceUri"]
    ?? builder.Configuration["AzureWebJobsStorage__tableServiceUri"]
    ?? Environment.GetEnvironmentVariable("AzureWebJobsStorage__tableServiceUri");

    if (string.IsNullOrWhiteSpace(uriString))
    {
        throw new InvalidOperationException(
            "Missing configuration: 'AzureWebJobsStorage__tableServiceUri'. This must be set in the environment variables or App Settings."
        );
    }
    builder.Services.AddSingleton(new TableServiceClient(new Uri(uriString), new DefaultAzureCredential()));
    
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