using api.Helpers;
using api.Models.Requests;
using api.Models.Responses;
using api.Models.Storage;
using api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;

namespace api.Extensions;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/ask", Ask).WithName("ask").RequireAuthorization("AccessAsUser");
    }

    internal static async Task<Results<Ok<AskResponse>, NotFound<string>>> Ask(
        HttpContext httpContext, 
        AskRequest req, 
        IOpenAiService aiService, 
        IDbService dbService,
        ILogger<Program> logger)
    {
        Console.WriteLine("Executing Ask endpoint...");

        string userQuestion = req.Question;
        float[] embeddedQuestion;

        embeddedQuestion = await aiService.EmbedAsync(userQuestion);

        var quotes = await dbService.GetAllQuotesAsync();
        if (quotes == null || quotes.Count == 0)
        {
            return TypedResults.NotFound("No quotes available.");
        }

        // Deserialize embeddings and find best match
        float bestScore = float.NegativeInfinity;
        TableData? bestQuote = null;

        foreach (var q in quotes)
        {
            var quoteEmbedding = JsonSerializer.Deserialize<float[]>(q.OpenAi3SEmbeddingJson);
            if (quoteEmbedding == null || quoteEmbedding.Length != embeddedQuestion.Length)
            {
                logger.LogWarning("Skipping quote with Rowkey = {RowKey} due to invalid embedding length.", q.RowKey);
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
            return TypedResults.NotFound("No matching quote found.");

        //TODO: Save quote linked to user to not repeat:
        //(If Table Storage)
        //UserQuotes

        //PartitionKey = userId

        //RowKey = quoteId

        //Properties: ShownOn, IsFavorite

        //also check shownon, and if long ago, then show again and update shownon

        return TypedResults.Ok(new AskResponse(
            bestQuote.QuoteString,
            bestQuote.PartitionKey,
            bestScore
        ));
    }
    
}


