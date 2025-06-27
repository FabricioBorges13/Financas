public interface ITransacaoRepository
{
    Task AdicionarAsync(Transacao transacao);
    Task<bool> ExistePorChaveIdempotenciaAsync(Guid chave);
    Task<Transacao?> ObterPorIdAsync(Guid id);
}