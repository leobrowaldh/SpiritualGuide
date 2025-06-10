using Azure;
using Azure.Data.Tables;
using System.Security.Cryptography;
using System.Text;
namespace FunctionApp.Models.Storage;

public class TableData : ITableEntity
{
    public string QuoteString { get; set; } = string.Empty;
    public string OpenAi3SEmbeddingJson { get; set; } = string.Empty;
    public string OpenAi3LEmbeddingJson { get; set; } = string.Empty;
    public string HFe5bEmbeddingJson { get; set; } = string.Empty;
    public string HFe5lEmbeddingJson { get; set; } = string.Empty;
    public string HFminiLMEmbeddingJson { get; set; } = string.Empty;
    public string HFdistiluseEmbeddingJson { get; set; } = string.Empty;

    //corresponds to the author of the quote:
    public string PartitionKey { get; set; } = string.Empty;
    //Corresponds to the normalized hashed quote:
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    //Allways invoke this when creating new entity
    public void SetKeys()
    {
        var normalized = Normalize(QuoteString);
        var hash = ComputeHash(normalized);
        RowKey = hash; // use hash as row key
    }

    private static string Normalize(string input) => input.Trim().ToLowerInvariant();

    private static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}

