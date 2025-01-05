namespace Application.Contracts.Infrastructure;

public interface IRedisRepository
{
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    Task<bool> RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task<bool> KeyPersistAsync(string key);
    Task<TimeSpan?> GetKeyTimeToLiveAsync(string key);
    Task<bool> ClearAllAsync();
}