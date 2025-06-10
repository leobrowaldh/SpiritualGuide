using Microsoft.Extensions.Logging;
using OpenAI.Embeddings;

namespace FunctionApp.Services;

public class OpenAiService : IOpenAiService
{
    private readonly EmbeddingClient _client;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(EmbeddingClient client, ILogger<OpenAiService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<List<float[]>> EmbedAsync(IEnumerable<string> quotes)
    {
        try
        {
            var result = await _client.GenerateEmbeddingsAsync(quotes);

            _logger.LogInformation("Successfully generated embeddings for {value count} quotes.", result.Value.Count);

            return result.Value.Select(r => r.ToFloats().ToArray()).ToList();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling OpenAI embedding API");
            throw;
        }
    }

    public async Task<float[]> EmbedAsync(string quote)
    {
        try
        {
            var result = await _client.GenerateEmbeddingAsync(quote);

            _logger.LogInformation("Successfully embedded user question.");

            return result.Value.ToFloats().ToArray();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling OpenAI embedding API");
            throw;
        }
    }
}
