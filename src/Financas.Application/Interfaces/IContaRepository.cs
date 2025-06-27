public interface IContaRepository
{
    Task AdicionarAsync(Conta conta);
    Task<Conta?> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(Conta conta);
    Task<bool> ExisteNumeroContaAsync(long numeroConta);
    Task<List<ContaDTO>> BuscarContasAsync();
    Task<Conta?> BuscarPorNumeroConta(long numeroConta);
}