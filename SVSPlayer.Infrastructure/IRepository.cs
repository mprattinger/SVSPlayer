using Azure.Data.Tables;

namespace SVSPlayer.Infrastructure;

public interface IRepository<T>
{
    void SetPartitionKey(string key);
    string GetPartitionKey();
    public TableClient TableClient { get; init; }
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByKeyAsync(string key);
    Task<IEnumerable<T>> GetWithFilterAsync(string filter);
    Task AddOrUpdateAsync(T item);
    Task AddMultipleAsync(IEnumerable<T> items);
    Task RemoveAsync(T item);
    Task RemoveByKeyAsync(string key);
}