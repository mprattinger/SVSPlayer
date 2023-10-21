using Azure;
using Azure.Data.Tables;

namespace SVSPlayer.Infrastructure;

public class TableClientRepository<T> : IRepository<T> where T : EntityBase, new()
{
    public TableClient TableClient { get; init; }

    public string PartitionKey { get; private set; } = "";

    public TableClientRepository(TableServiceClient tableServiceClient)
    {
        TableClient = tableServiceClient.GetTableClient(typeof(T).Name);
    }

    public void SetPartitionKey(string key)
    {
        PartitionKey = key;
    }

    public string GetPartitionKey()
    {
        return PartitionKey;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        await ensureCreated();

        checkPartitionKey();

        return await Task.Run(() =>
        {
            Pageable<T> result = TableClient.Query<T>(filter: $"PartitionKey eq '{PartitionKey}'");

            return result;
        });
    }

    public async Task<T?> GetByKeyAsync(string key)
    {
        await ensureCreated();

        checkPartitionKey();

        return await Task.Run(() =>
        {
            Pageable<T> result = TableClient.Query<T>(filter: $"PartitionKey eq '{PartitionKey}' and RowKey eq '{key}'");

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<T>> GetWithFilterAsync(string filter)
    {
        await ensureCreated();

        checkPartitionKey();

        var currFilter = $"PartitionKey eq '{PartitionKey}'";
        currFilter = $"{currFilter} and {filter}";

        return await Task.Run(() =>
        {
            Pageable<T> result = TableClient.Query<T>(filter: currFilter);

            return result;
        });
    }


    public async Task AddOrUpdateAsync(T item)
    {
        await ensureCreated();

        checkPartitionKey();

        var update = true;

        if (string.IsNullOrEmpty(item.PartitionKey))
        {
            item.PartitionKey = PartitionKey;
        }
        
        if (string.IsNullOrEmpty(item.RowKey))
        {
            item.RowKey = Guid.NewGuid().ToString("N");
            update = false;
        }

        var ret = await TableClient.UpsertEntityAsync<T>(item, TableUpdateMode.Replace);

        if (ret.IsError)
        {
            if (update)
            {
                throw new Exception($"Error updating entity {typeof(T).Name}! ({ret.Status} - {ret.ReasonPhrase})");
            }
            throw new Exception($"Error creating entity {typeof(T).Name}! ({ret.Status} - {ret.ReasonPhrase})");
        }
    }

    public async Task AddMultipleAsync(IEnumerable<T> items)
    {
        await ensureCreated();

        checkPartitionKey();

        foreach (var i in items)
        {
            i.PartitionKey = PartitionKey;
            i.RowKey = Guid.NewGuid().ToString();
        }

        List<TableTransactionAction> addEntitiesBatch = new List<TableTransactionAction>();
        addEntitiesBatch.AddRange(items.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));
        Response<IReadOnlyList<Response>> response = await TableClient.SubmitTransactionAsync(addEntitiesBatch).ConfigureAwait(false);

        if (response.Value.Any(x => x.IsError))
        {
            var firstError = response.Value.First(x => x.IsError);
            throw new Exception($"Error creating entity {typeof(T).Name}! ({firstError.Status} - {firstError.ReasonPhrase})");
        }
    }

    public async Task RemoveAsync(T item)
    {
        await ensureCreated();

        checkPartitionKey();

        await TableClient.DeleteEntityAsync(item.PartitionKey, item.RowKey);
    }

    public async Task RemoveByKeyAsync(string key)
    {
        await ensureCreated();

        checkPartitionKey();

        var ret = await TableClient.DeleteEntityAsync(PartitionKey, key);

        if (ret.IsError)
        {
            throw new Exception($"Error deleting {typeof(T).Name} with rowkey {key}! ({ret.Status} - {ret.ReasonPhrase})");
        }
    }

    private void checkPartitionKey()
    {
        if (String.IsNullOrEmpty(PartitionKey))
        {
            throw new Exception("No partition key set for repo");
        }
    }
    private async Task ensureCreated()
    {
        await TableClient.CreateIfNotExistsAsync();
    }

}