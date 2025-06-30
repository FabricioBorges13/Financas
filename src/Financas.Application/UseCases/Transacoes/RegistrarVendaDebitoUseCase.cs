
public class RegistrarVendaDebitoUseCase : IRegistrarVendaDebitoUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;

    public RegistrarVendaDebitoUseCase(IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
    }
    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaDebitoRequest request, CancellationToken cancellationToken)
    {
        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.VendaDebito, request.ContaId, valor: request.Valor);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            var conta = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (conta == null)
                throw new InvalidOperationException($"Conta {request.ContaId} não encontrada!");

            conta.RegistrarVenda(request.Valor, TipoTransacao.VendaDebito);

            var transacao = new Transacao(conta.Id, request.Valor, TipoTransacao.VendaDebito);

            await _contaRepository.AtualizarAsync(conta);

            await _transacaoRepository.AdicionarAsync(transacao);
            await _auditoriaService.RegistrarAsync("conta", conta.Id,
                    dados: $"Compra de R${transacao.Valor} registrada",
                    TipoTransacao.VendaDebito,
                    StatusTransacao.Concluida);

            return new RegistrarVendaResponse
            {
                DataHora = DateTime.Now,
                SaldoDisponivel = conta.SaldoDisponivel,
                Tipo = TipoTransacao.VendaDebito,
                ContaId = conta.Id,
                TransacaoId = transacao.Id
            };
        }, cancellationToken,
        onFailure: async ex =>
        {
            await _auditoriaService.RegistrarAsync("conta", request.ContaId,
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.VendaDebito,
            StatusTransacao.Falhou);
        });
    }
}