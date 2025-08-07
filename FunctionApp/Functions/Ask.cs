using Azure;
using FunctionApp.Helpers;
using FunctionApp.Models.Storage;
using FunctionApp.Services;
using Microsoft.AspNetCore.Authorization;
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

    //for now i have to use this since CORS doesnt handle preflight options request in function apps, damn it
    [Function(nameof(Ask))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", "options")] HttpRequestData req)
    {
        _logger.LogInformation("Processing request: {Method}", req.Method);

        if (req.Method == HttpMethods.Options)
        {
            var preflight = req.CreateResponse(HttpStatusCode.NoContent);
            preflight.Headers.Add("Access-Control-Allow-Origin", "https://zealous-water-0ccf68303.1.azurestaticapps.net");
            preflight.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
            preflight.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            preflight.Headers.Add("Access-Control-Allow-Credentials", "true");
            return preflight;
        }

        string userQuestion = await new StreamReader(req.Body).ReadToEndAsync();

        float[] embeddedQuestion;
        try
        {
            embeddedQuestion = await _aiService.EmbedAsync(userQuestion);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("Database call did not go as expected.");
            notFound.Headers.Add("Access-Control-Allow-Origin", "https://zealous-water-0ccf68303.1.azurestaticapps.net");
            return notFound;
        }

        var quotes = await _dbService.GetAllQuotesAsync();
        if (quotes == null || quotes.Count == 0)
        {
            var noQuotes = req.CreateResponse(HttpStatusCode.NotFound);
            await noQuotes.WriteStringAsync("No quotes available.");
            noQuotes.Headers.Add("Access-Control-Allow-Origin", "https://zealous-water-0ccf68303.1.azurestaticapps.net");
            return noQuotes;
        }

        float bestScore = float.NegativeInfinity;
        TableData? bestQuote = null;

        foreach (var q in quotes)
        {
            var quoteEmbedding = JsonSerializer.Deserialize<float[]>(q.OpenAi3SEmbeddingJson);
            if (quoteEmbedding == null || quoteEmbedding.Length != embeddedQuestion.Length)
                continue;

            float score = AiHelper.CosineSimilarity(embeddedQuestion, quoteEmbedding);
            if (score > bestScore)
            {
                bestScore = score;
                bestQuote = q;
            }
        }

        if (bestQuote == null)
        {
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync("No matching quote found.");
            notFound.Headers.Add("Access-Control-Allow-Origin", "https://zealous-water-0ccf68303.1.azurestaticapps.net");
            return notFound;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            Quote = bestQuote.QuoteString,
            Author = bestQuote.PartitionKey,
            Similarity = bestScore
        });
        response.Headers.Add("Access-Control-Allow-Origin", "https://zealous-water-0ccf68303.1.azurestaticapps.net");
        response.Headers.Add("Access-Control-Allow-Credentials", "true");

        return response;
    }


    //use this in asp.net api:
    //[Function(nameof(Ask))]
    //public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    //{
    //    _logger.LogInformation("C# HTTP trigger function processed a request.");

    //    string userQuestion = await new StreamReader(req.Body).ReadToEndAsync();
    //    float[] embeddedQuestion;
    //    try
    //    {
    //        embeddedQuestion = await _aiService.EmbedAsync(userQuestion);
    //    }
    //    catch (RequestFailedException ex) when (ex.Status == 404)
    //    {
    //        _logger.LogWarning("Table Storage Issues: {Message}", ex.Message);
    //        return new NotFoundObjectResult("Database call did not go as expected.");
    //    }
    //    var quotes = await _dbService.GetAllQuotesAsync();
    //    if (quotes == null || quotes.Count == 0)
    //    {
    //        return new NotFoundObjectResult("No quotes available.");
    //    }

    //    // Deserialize embeddings and find best match
    //    float bestScore = float.NegativeInfinity;
    //    TableData? bestQuote = null;

    //    foreach (var q in quotes)
    //    {
    //        var quoteEmbedding = JsonSerializer.Deserialize<float[]>(q.OpenAi3SEmbeddingJson);
    //        if (quoteEmbedding == null || quoteEmbedding.Length != embeddedQuestion.Length)
    //        {
    //            _logger.LogWarning("Skipping quote with Rowkey = {RowKey} due to invalid embedding length.", q.RowKey);
    //            continue;
    //        }

    //        float score = AiHelper.CosineSimilarity(embeddedQuestion, quoteEmbedding);
    //        if (score > bestScore)
    //        {
    //            bestScore = score;
    //            bestQuote = q;
    //        }
    //    }

    //    if (bestQuote == null)
    //        return new NotFoundObjectResult("No matching quote found.");

    //    //TODO: Save quote linked to user to not repeat:
    //    //(If Table Storage)
    //    //UserQuotes

    //    //PartitionKey = userId

    //    //RowKey = quoteId

    //    //Properties: ShownOn, IsFavorite

    //    //also check shownon, and if long ago, then show again and update shownon

    //    return new OkObjectResult(new
    //    {
    //        Quote = bestQuote.QuoteString,
    //        Author = bestQuote.PartitionKey,
    //        Similarity = bestScore
    //    });
    //}
}