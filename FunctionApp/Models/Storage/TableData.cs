using Azure;
using Azure.Data.Tables;
namespace FunctionApp.Models.Storage;

public class TableData : ITableEntity
{
    public string QuoteString { get; set; } = string.Empty;
    public string EmbeddingJson { get; set; } =string.Empty;
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
