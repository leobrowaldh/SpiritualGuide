using FunctionApp.Helpers;
using FunctionApp.Models.Storage;
using FunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
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
    public async Task<HttpResponseData> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
    FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(Ask));
        logger.LogInformation("Processing question...");

        string userQuestion = await new StreamReader(req.Body).ReadToEndAsync();
        float[] embeddedQuestion = await _aiService.EmbedAsync(userQuestion);

        var quotes = await _dbService.GetAllQuotesAsync();

        float bestScore = float.NegativeInfinity;
        TableData? bestQuote = null;

        foreach (var q in quotes)
        {
            var quoteEmbedding = JsonSerializer.Deserialize<float[]>(q.OpenAi3SEmbeddingJson);
            if (quoteEmbedding == null || quoteEmbedding.Length != embeddedQuestion.Length)
            {
                logger.LogWarning("Invalid embedding length for RowKey = {RowKey}", q.RowKey);
                continue;
            }

            float score = AiHelper.CosineSimilarity(embeddedQuestion, quoteEmbedding);
            if (score > bestScore)
            {
                bestScore = score;
                bestQuote = q;
            }
        }

        var response = req.CreateResponse();

        if (bestQuote == null)
        {
            response.StatusCode = HttpStatusCode.NotFound;
            await response.WriteStringAsync("No matching quote found.");
            //TODO: Save quote linked to user to not repeat:
            //(If Table Storage)
            //UserQuotes

            //PartitionKey = userId

            //RowKey = quoteId

            //Properties: ShownOn, IsFavorite

            //also check shownon, and if long ago, then show again and update shownon
        }
        else
        {
            response.StatusCode = HttpStatusCode.OK;
            var json = JsonSerializer.Serialize(new
            {
                Quote = bestQuote.QuoteString,
                Author = bestQuote.PartitionKey,
                Similarity = bestScore
            });
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("X-Content-Type-Options", "nosniff");
            response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate");
            await response.WriteStringAsync(json);
        }

        return response;
    }
}