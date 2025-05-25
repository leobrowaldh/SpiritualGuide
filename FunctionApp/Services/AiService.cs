using Microsoft.Extensions.Logging;
using OpenAI.Embeddings;

namespace FunctionApp.Services;

public class AiService : IAiService
{
    private readonly EmbeddingClient _client;
    private readonly ILogger<AiService> _logger;

    public AiService(EmbeddingClient client, ILogger<AiService> logger)
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

}
