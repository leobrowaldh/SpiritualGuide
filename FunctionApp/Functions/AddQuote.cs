using FunctionApp.Enums;
using FunctionApp.Models.FunctionRequestModels;
using FunctionApp.Models.FunctionReturnModels;
using FunctionApp.Models.Storage;
using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionApp.Functions;

public class AddQuote
{
    private readonly ILogger<AddQuote> _logger;
    private readonly IAiService _aiService;

    public AddQuote(ILogger<AddQuote> logger, IAiService aiService)
    {
        _logger = logger;
        _aiService = aiService;
    }

    [Function(nameof(AddQuote))]
    public async Task<AddQuoteReturnModel> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Http trigger - Adding Quote");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var request = JsonSerializer.Deserialize<AddQuoteRequest>(requestBody);

        if (request == null || request.Quotes == null || request.Quotes.Count == 0)
        {
            throw new ArgumentException("Invalid request body.");
        }

        if (!Enum.IsDefined(typeof(EnAuthor), request.Author))
        {
            var validAuthors = string.Join(", ", Enum.GetValues<EnAuthor>());
            throw new ArgumentException($"Invalid author. Valid values are: {validAuthors}");
        }

        string author = request.Author.ToString();
        List<string> quotes = request.Quotes;

        _logger.LogInformation("Received {quotes count} quotes for embedding.", quotes.Count);

        _logger.LogInformation("Calling OpenAI embedding API to embed quotes.");
        var embeddedQuotes = await _aiService.EmbedAsync(quotes);

        List<TableData> tableDatas = [];

        for (int i = 0; i < embeddedQuotes.Count; i++)
        {
            var data = new TableData
            {
                QuoteString = quotes[i],
                EmbeddingJson = JsonSerializer.Serialize(embeddedQuotes[i]),
                PartitionKey = $"{author}"
            };

            data.SetKeys(); // compute Hash and RowKey
            tableDatas.Add(data);
        }

        _logger.LogInformation("Saving {tableDatas Count} quotes to storage.", tableDatas.Count);
        return new AddQuoteReturnModel() { ActionResult = new OkObjectResult(tableDatas), TableDatas = tableDatas };
    }
}