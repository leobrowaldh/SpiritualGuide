using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp.Services;
internal class DbService : IDbService
{
    private readonly TableClient _quoteTableClient;

    public DbService(IAzureClientFactory<TableServiceClient> tableClientFactory)
    {
        _quoteTableClient = tableClientFactory.CreateClient("QuotesTableClient").GetTableClient("Quotes");
        _quoteTableClient.CreateIfNotExists(); //creates the table if it doesnt yet exist.
    }
    public async Task AddTableEntitiesAsync(List<ITableEntity> entities)
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
}
