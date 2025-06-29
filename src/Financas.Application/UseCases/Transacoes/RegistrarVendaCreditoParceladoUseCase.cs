
public class RegistrarVendaCreditoParceladoUseCase : IRegistrarVendaCreditoParceladoUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;

    public RegistrarVendaCreditoParceladoUseCase(IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
    }
    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaCreditoParceladoRequest request, CancellationToken cancellationToken)
    {

        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.VendaCreditoParcelado, request.ContaId, valor: request.Valor);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            var conta = await _contaRepository.BuscarPorNumeroConta(request.NumeroConta);
            if (conta == null)
                throw new InvalidOperationException($"Conta {request.NumeroConta} não encontrada!");

            conta.RegistrarVenda(request.Valor, TipoTransacao.VendaCreditoParcelado);

            var transacao = new Transacao(conta.Id, request.Valor, TipoTransacao.VendaCreditoParcelado, parcelas: request.Parcelas);

            await _contaRepository.AtualizarAsync(conta);

            await _transacaoRepository.AdicionarAsync(transacao);

            await _auditoriaService.RegistrarAsync("conta",
                    dados: $"Compra de R${transacao.Valor} registrada em {transacao.Parcelas} parcelas",
                    TipoTransacao.VendaCreditoParcelado,
                    StatusTransacao.Concluida);

            return new RegistrarVendaResponse
            {
                DataHora = DateTime.Now,
                SaldoFuturo = conta.SaldoFuturo,
                Tipo = TipoTransacao.VendaCreditoParcelado,
                ContaId = conta.Id,
                TransacaoId = transacao.Id
            };
        }, cancellationToken,
        onFailure: async ex =>
        {
            await _auditoriaService.RegistrarAsync("conta",
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.VendaCreditoParcelado,
            StatusTransacao.Falhou);
        });
    }
}