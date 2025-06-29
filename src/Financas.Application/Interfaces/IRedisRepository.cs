public interface IRedisRepository
{
    Task<bool> ExistsAsync(string key);
    Task SetAsync(string key, string value, TimeSpan expiration);
    Task<bool> SetIfNotExistsAsync(string key, string value, TimeSpan expiry);
    Task<bool> ReleaseIfMatchAsync(string key, string value);
}