using Azure;
using FunctionApp.Helpers;
using FunctionApp.Models.Storage;
using FunctionApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace FunctionApp.Functions;

public class Ask
{
    private readonly ILogger<Ask> _logger;
    private readonly IDbService _dbService;
    private readonly IOpenAiService _aiService;

    public Ask(ILogger<Ask> logger, IDbService dbService, IOpenAiService aiService)
    {
        _logger = logger;
        _dbService = dbService;
        _aiService = aiService;
    }

    [Function(nameof(Ask))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string userQuestion = await new StreamReader(req.Body).ReadToEndAsync();
        float[] embeddedQuestion;
        try
        {
            embeddedQuestion = await _aiService.EmbedAsync(userQuestion);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogWarning("Table Storage Issues: {Message}", ex.Message);
            return new NotFoundObjectResult("Database call did not go as expected.");
        }
        var quotes = await _dbService.GetAllQuotesAsync();
        if (quotes == null || quotes.Count == 0)
        {
            return new NotFoundObjectResult("No quotes available.");
        }

        // Deserialize embeddings and find best match
        float bestScore = float.NegativeInfinity;
        TableData? bestQuote = null;

        foreach (var q in quotes)
        {
            var quoteEmbedding = JsonSerializer.Deserialize<float[]>(q.OpenAi3SEmbeddingJson);
            if (quoteEmbedding == null || quoteEmbedding.Length != embeddedQuestion.Length)
            {
                _logger.LogWarning("Skipping quote with Rowkey = {RowKey} due to invalid embedding length.", q.RowKey);
                continue;
            }

            float score = AiHelper.CosineSimilarity(embeddedQuestion, quoteEmbedding);
            if (score > bestScore)
            {
                bestScore = score;
                bestQuote = q;
            }
        }

        if (bestQuote == null)
            return new NotFoundObjectResult("No matching quote found.");

        //TODO: Save quote linked to user to not repeat:
        //(If Table Storage)
        //UserQuotes

        //PartitionKey = userId

        //RowKey = quoteId

        //Properties: ShownOn, IsFavorite

        //also check shownon, and if long ago, then show again and update shownon

        return new OkObjectResult(new
        {
            Quote = bestQuote.QuoteString,
            Author = bestQuote.PartitionKey,
            Similarity = bestScore
        });
    }
}