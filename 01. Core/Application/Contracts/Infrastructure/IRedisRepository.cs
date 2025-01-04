namespace Application.Contracts.Infrastructure;

public interface IRedisRepository
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> ClearAllAsync();
}