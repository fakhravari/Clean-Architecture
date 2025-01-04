﻿using Application.Contracts.Infrastructure;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Redis;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _database;

    public RedisRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
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

    public async Task<bool> ClearAllAsync()
    {
        var endPoint = _database.Multiplexer.GetEndPoints().First();
        var server = _database.Multiplexer.GetServer(endPoint);

        var keys = server.Keys(_database.Database).ToArray();
        foreach (var key in keys)
        {
            await _database.KeyDeleteAsync(key);
        }

        return true;
    }
}