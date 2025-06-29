
public class AdicionarSaldoUseCase : IAdicionarSaldoUseCase
{
    private readonly IContaRepository _contaRepository;

    public AdicionarSaldoUseCase(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }
    public async Task<ContaDTO> ExecutarAsync(AdicionarSaldoRequest request)
    {
        var conta = await _contaRepository.BuscarPorNumeroConta(request.NumeroConta);
        if (conta == null)
            throw new InvalidOperationException($"Conta {request.NumeroConta} não encontrada!");

        conta.AdicionarSaldo(request.Saldo);

        await _contaRepository.AtualizarAsync(conta);

        return new ContaDTO
        {
            NumeroConta = conta.NumeroConta,
            SaldoDisponivel = conta.SaldoDisponivel,
            Id = conta.Id,
            DataAbertura = conta.DataAbertura,
            DataEncerramento = conta.DataEncerramento,
            SaldoBloqueado = conta.SaldoBloqueado,
            SaldoFuturo = conta.SaldoFuturo,
            Status = conta.Status,
            TipoConta = conta.TipoConta
        };
    }
}