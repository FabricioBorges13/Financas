
public class BuscarContaUseCase : IBuscarContaUseCase
{
    private readonly IContaRepository _contaRepositor;
    public BuscarContaUseCase(IContaRepository contaRepository)
    {
        _contaRepositor = contaRepository;
    }
    public async Task<ContaDTO> ExecutarAsync(long numeroConta)
    {
        var conta = await _contaRepositor.BuscarPorNumeroConta(numeroConta);

        if (conta == null)
            throw new KeyNotFoundException("Conta não encontrada");

        return new ContaDTO
        {
            DataAbertura = conta.DataAbertura,
            DataEncerramento = conta.DataEncerramento,
            Id = conta.Id,
            NumeroConta = conta.NumeroConta,
            SaldoBloqueado = conta.SaldoBloqueado,
            SaldoDisponivel = conta.SaldoDisponivel,
            SaldoFuturo = conta.SaldoFuturo,
            Status = conta.Status,
            TipoConta = conta.TipoConta,
            Cliente = conta.Cliente
        };
    }
}