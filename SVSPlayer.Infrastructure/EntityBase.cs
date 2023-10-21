using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace SVSPlayer.Infrastructure;

public class EntityBase : ITableEntity
{
    public string PartitionKey { get; set; } = "";
    public string RowKey { get; set; } = "";
    public DateTimeOffset? Timestamp { get; set; }
    [JsonIgnore]
    public ETag ETag { get; set; }
}