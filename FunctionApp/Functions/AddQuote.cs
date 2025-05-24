using FunctionApp.Models.FunctionReturnModels;
using FunctionApp.Models.Storage;
using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Tables;
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
        var incomingQuotes = JsonSerializer.Deserialize<List<string>>(requestBody);

        if (incomingQuotes == null || !incomingQuotes.Any())
        {
            _logger.LogError("No quotes provided in the request body.");
            return new AddQuoteReturnModel(new BadRequestObjectResult("No quotes provided."), new List<TableData>());
        }
        _logger.LogInformation($"Received {incomingQuotes.Count} quotes for embedding.");

        _logger.LogInformation("Calling OpenAI embedding API to embed quotes.");
        var embeddedQuotes = await _aiService.EmbedAsync(incomingQuotes);

        List<TableData> tableDatas = embeddedQuotes.Select((embedding, index) => new TableData
        {
            QuoteString = incomingQuotes[index],
            EmbeddingJson = JsonSerializer.Serialize(embedding),
            PartitionKey = "quote",
            RowKey = Guid.NewGuid().ToString()
        }).ToList();

        _logger.LogInformation($"Saving {tableDatas.Count} quotes to storage.");
        return new AddQuoteReturnModel(new OkObjectResult(tableDatas), tableDatas);
    }
}