using FunctionApp.Enums;
using FunctionApp.Models.FunctionRequestModels;
using FunctionApp.Models.Storage;
using FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace FunctionApp.Functions;

public class AddQuote
{
    private readonly ILogger<AddQuote> _logger;
    private readonly IOpenAiService _openAiService;
    private readonly IDbService _dbService;

    public AddQuote(ILogger<AddQuote> logger, IOpenAiService openAiService, IDbService dbService)
    {
        _logger = logger;
        _openAiService = openAiService;
        _dbService = dbService;
    }

    [Function(nameof(AddQuote))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext context)
    {
        _logger.LogInformation("Http trigger - Adding Quote");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var request = JsonSerializer.Deserialize<AddQuoteRequest>(requestBody);

        if (request == null || request.Quotes == null || request.Quotes.Count == 0)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request body.");
            return badResponse;
        }

        if (!Enum.IsDefined(typeof(EnAuthor), request.Author))
        {
            var validAuthors = string.Join(", ", Enum.GetValues<EnAuthor>());
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync($"Invalid author. Valid values are: {validAuthors}");
            return badResponse;
        }

        string author = request.Author.ToString();
        List<string> quotes = request.Quotes;

        _logger.LogInformation("Received {quotes count} quotes for embedding.", quotes.Count);

        _logger.LogInformation("Calling OpenAI embedding API to embed quotes.");
        var embeddedQuotes = await _openAiService.EmbedAsync(quotes);

        List<TableData> tableDatas = [];

        for (int i = 0; i < embeddedQuotes.Count; i++)
        {
            var data = new TableData
            {
                QuoteString = quotes[i],
                OpenAi3SEmbeddingJson = JsonSerializer.Serialize(embeddedQuotes[i]),
                PartitionKey = $"{author}"
            };

            data.SetKeys();
            tableDatas.Add(data);
        }

        _logger.LogInformation("Saving {tableDatas Count} quotes to storage.", tableDatas.Count);
        await _dbService.AddTableEntitiesAsync(tableDatas);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        response.Headers.Add("X-Content-Type-Options", "nosniff");
        response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
        await response.WriteStringAsync(JsonSerializer.Serialize(tableDatas));

        return response;
    }
}


//Ideal Chunk Size for Embedding
//Length: ~30 to 90 seconds of spoken content

//Words: ~75 to 225 words

//Why: This roughly corresponds to 500–750 tokens, which is well within the sweet spot for models like OpenAI’s text-embedding-3-small.

