public interface ITransacaoRepository
{
    Task AdicionarAsync(Transacao transacao);
    Task<Transacao?> ObterPorIdAsync(Guid id);
}