
public class RedisService : IRedisService
{
    private readonly IRedisRepository _redisRepository;
    public RedisService(IRedisRepository redisRepository)
    {
        _redisRepository = redisRepository;
    }
    public async Task<bool> ExistsAsync(string key)
    {
        return await _redisRepository.ExistsAsync(key);
    }

    public async Task<bool> ReleaseIfMatchAsync(string key, string value)
    {
        return await _redisRepository.ReleaseIfMatchAsync(key, value);
    }

    public async Task SetAsync(string key, string value, TimeSpan expiration)
    {
        await _redisRepository.SetAsync(key, value, expiration);
    }

    public async Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiry)
    {
        return await _redisRepository.SetIfNotExistsAsync(key, value, expiry);
    }
}