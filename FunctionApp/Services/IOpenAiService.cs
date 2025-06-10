
namespace FunctionApp.Services;

public interface IOpenAiService
{
    /// <summary>
    /// create a list of embeddings for the given quotes using OpenAI's embedding API
    /// </summary>
    /// <param name="quotes"></param>
    /// <returns></returns>
    Task<List<float[]>> EmbedAsync(IEnumerable<string> quotes);
    Task<float[]> EmbedAsync(string quote);
}
