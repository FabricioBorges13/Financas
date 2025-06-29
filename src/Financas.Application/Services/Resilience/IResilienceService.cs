public interface IResilienceService
{
    public Task<T> ExecuteAsync<T>(string chaveLock, string chaveIdempotencia, Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken, Func<Exception, Task>? onFailure = null);
}