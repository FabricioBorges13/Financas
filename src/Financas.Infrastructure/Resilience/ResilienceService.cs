using Polly;

public class ResilienceService : IResilienceService
{
    private readonly AppDbContext _dbContext;
    private readonly IRedisService _cache;

    public ResilienceService(AppDbContext dbContext, IRedisService cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<T> ExecuteAsync<T>(string chaveLock,
    string chaveIdempotencia,
    Func<CancellationToken, Task<T>> action,
    CancellationToken cancellationToken,
    Func<Exception, Task>? onFailure = null)
    {
        // Idempotência
        if (await _cache.ExistsAsync(chaveIdempotencia))
            throw new InvalidOperationException("Transação já processada.");

        // Lock distribuído
        var lockToken = Guid.NewGuid().ToString();
        var lockAcquired = await _cache.SetIfNotExistsAsync(chaveLock, lockToken, TimeSpan.FromSeconds(30));

        if (!lockAcquired)
            throw new InvalidOperationException("Recurso em uso. Tente novamente.");

        try
        {
            // Polly: Retry + Fallback
            var fallbackPolicy = Policy<T>
                .Handle<Exception>()
                .FallbackAsync(default(T), async (ex, ct) =>
                {
                    if (onFailure is not null)
                        await onFailure(ex.Exception);
                });

            var retryPolicy = Policy<T>
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(300));

            var policy = Policy.WrapAsync(fallbackPolicy, retryPolicy);

            return await policy.ExecuteAsync(async ct =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
                try
                {
                    T resultado = await action(ct);

                    await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    await _cache.SetAsync(chaveIdempotencia, "executada", TimeSpan.FromMinutes(5));

                    return resultado;
                }
                catch (Exception ex)
                {
                    if (onFailure is not null)
                        await onFailure(ex);
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            }, cancellationToken);
        }
        finally
        {
            await _cache.ReleaseIfMatchAsync(chaveLock, lockToken);
        }
    }
}