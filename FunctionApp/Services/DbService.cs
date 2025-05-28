using Azure;
using Azure.Data.Tables;
using FunctionApp.Models.Storage;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FunctionApp.Services;
internal class DbService : IDbService
{
    private readonly TableClient _quoteTableClient;
    private readonly ILogger<DbService> _logger;

    public DbService(IAzureClientFactory<TableServiceClient> tableClientFactory, ILogger<DbService> logger)
    {
        _quoteTableClient = tableClientFactory.CreateClient("QuotesTableClient").GetTableClient("Quotes");
        _quoteTableClient.CreateIfNotExists(); //creates the table if it doesnt yet exist.
        _logger = logger;
    }
    public async Task AddTableEntitiesAsync(List<TableData> entities)
    {
        foreach (var entity in entities)
        {
            try
            {
                //await each instead of awaitall, to avoid throttling conditions in a non performat demanding admin task.
                await _quoteTableClient.AddEntityAsync(entity);
            }
            catch (RequestFailedException ex) when (ex.Status == 409)
            {
                // Entity already exists — skip
            }
        }
    }

    public async Task<List<TableData>> GetAllQuotesAsync()
    {
        var entities = new List<TableData>();

        // QueryAsync without a filter fetches all entities, paged internally.
        await foreach (TableData entity in _quoteTableClient.QueryAsync<TableData>())
        {
            entities.Add(entity);
        }

        return entities;

    }
}
