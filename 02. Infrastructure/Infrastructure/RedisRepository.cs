using Application.Contracts.Infrastructure;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _database;

    public RedisRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        int IndexDb = 0;
        _database = connectionMultiplexer.GetDatabase(IndexDb);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _database.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<bool> KeyPersistAsync(string key)
    {
        return await _database.KeyPersistAsync(key);
    }

    public async Task<TimeSpan?> GetKeyTimeToLiveAsync(string key)
    {
        return await _database.KeyTimeToLiveAsync(key);
    }

    public async Task<bool> ClearAllAsync()
    {
        var endPoint = _database.Multiplexer.GetEndPoints().First();
        var server = _database.Multiplexer.GetServer(endPoint);

        var keys = server.Keys(_database.Database).ToArray();
        foreach (var key in keys) await _database.KeyDeleteAsync(key);

        return true;
    }
}