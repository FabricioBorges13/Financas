
public class AdicionarSaldoUseCase : IAdicionarSaldoUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;
     private readonly ITransacaoRepository _transacaoRepository;

    public AdicionarSaldoUseCase(IContaRepository contaRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService, ITransacaoRepository transacaoRepository)
    {
        _contaRepository = contaRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
        _transacaoRepository = transacaoRepository;
    }
    public async Task<ContaDTO> ExecutarAsync(AdicionarSaldoRequest request, CancellationToken cancellationToken)
    {
        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.AdicionarSaldo, request.ContaId, valor: request.Saldo);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            var conta = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (conta == null)
                throw new InvalidOperationException($"Conta {request.ContaId} não encontrada!");

            var transacao = new Transacao(conta.Id, request.Saldo, TipoTransacao.AdicionarSaldo);
            conta.AdicionarSaldo(request.Saldo);

            await _transacaoRepository.AdicionarAsync(transacao);
            await _contaRepository.AtualizarAsync(conta);
            await _auditoriaService.RegistrarAsync("conta", conta.Id,
                    dados: $"Adicionado saldo de R${request.Saldo}",
                    TipoTransacao.AdicionarSaldo,
                    StatusTransacao.Concluida);
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

        }, cancellationToken,
        onFailure: async ex =>
        {
            await _auditoriaService.RegistrarAsync("conta", request.ContaId, 
            dados: $"Falha ao adicionar saldo: {ex.Message}",
            TipoTransacao.AdicionarSaldo,
            StatusTransacao.Falhou);
        });
    }
}