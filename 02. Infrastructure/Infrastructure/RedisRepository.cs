using Application.Contracts.Infrastructure;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _db;

    public RedisRepository(IConnectionMultiplexer connectionMultiplexer, int defaultDbIndex = 0)
    {
        _db = connectionMultiplexer.GetDatabase(defaultDbIndex);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonConvert.SerializeObject(value);
        return await _db.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _db.StringGetAsync(key);
        return json.HasValue ? JsonConvert.DeserializeObject<T>(json) : default;
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    public async Task<bool> KeyPersistAsync(string key)
    {
        return await _db.KeyPersistAsync(key);
    }

    public async Task<TimeSpan?> GetKeyTimeToLiveAsync(string key)
    {
        return await _db.KeyTimeToLiveAsync(key);
    }

    public async Task<bool> ClearAllAsync()
    {
        var endPoint = _db.Multiplexer.GetEndPoints().First();
        var server = _db.Multiplexer.GetServer(endPoint);

        var keys = server.Keys(_db.Database).ToArray();
        foreach (var key in keys) await _db.KeyDeleteAsync(key);

        return true;
    }
}