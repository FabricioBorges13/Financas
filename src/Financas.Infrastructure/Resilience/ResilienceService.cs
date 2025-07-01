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
            // Fallback para exceções específicas que você já trata no controller
            var fallbackPolicy = Policy<T>
                .Handle<InvalidOperationException>()
                .Or<KeyNotFoundException>()
                .Or<ArgumentException>()
                .FallbackAsync(
                    fallbackAction: (context, ct) =>
                    {
                        // Rethrow a exceção preservada no contexto
                        if (context.TryGetValue("exception", out var exObj) && exObj is Exception ex)
                            throw ex;

                        throw new Exception("Erro desconhecido no fallback.");
                    },
                    onFallbackAsync: async (delegateResult, context) =>
                    {
                        var ex = delegateResult.Exception;

                        // Armazena exceção para rethrow no fallbackAction
                        context["exception"] = ex;

                        if (onFailure is not null)
                            await onFailure(ex);
                    });

            // Retry genérico
            var retryPolicy = Policy<T>
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(300));

            var policy = Policy.WrapAsync(fallbackPolicy, retryPolicy);

            // Executa política com contexto compartilhado
            var context = new Context(); // Polly.Context

            return await policy.ExecuteAsync(async (ctx, ct) =>
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
            }, context, cancellationToken);
        }
        finally
        {
            await _cache.ReleaseIfMatchAsync(chaveLock, lockToken);
        }
    }
}
