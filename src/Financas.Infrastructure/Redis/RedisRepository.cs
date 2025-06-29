using StackExchange.Redis;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _db;

    public RedisRepository(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }

    public Task<bool> ExistsAsync(string key) => _db.KeyExistsAsync(key);

    public Task SetAsync(string key, string value, TimeSpan expiration) =>
        _db.StringSetAsync(key, value, expiration);

    public async Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiry)
    {
        return await _db.StringSetAsync(key, value, expiry, When.NotExists);
    }

    public async Task<bool> ReleaseIfMatchAsync(string key, string value)
    {
        var tran = _db.CreateTransaction();
        tran.AddCondition(Condition.StringEqual(key, value));
        _ = tran.KeyDeleteAsync(key);
        return await tran.ExecuteAsync();
    }
}