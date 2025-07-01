
using Microsoft.Extensions.Logging;

public class RegistrarVendaCreditoParceladoUseCase : IRegistrarVendaCreditoParceladoUseCase
{
    private readonly ILogger<RegistrarVendaCreditoParceladoUseCase> _logger;
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;

    public RegistrarVendaCreditoParceladoUseCase(ILogger<RegistrarVendaCreditoParceladoUseCase> logger, IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
        _logger = logger;
    }
    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaCreditoParceladoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Gerando lock para venda credito a vista");
        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.VendaCreditoParcelado, request.ContaId, valor: request.Valor);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            _logger.LogInformation("Gerando lock para venda credito parcelado");
            var conta = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (conta == null)
            {
                _logger.LogInformation($"Conta {request.ContaId} não encontrada!");
                throw new InvalidOperationException($"Conta {request.ContaId} não encontrada!");
            }

            _logger.LogInformation($"Registrando venda!");
            conta.RegistrarVenda(request.Valor, TipoTransacao.VendaCreditoParcelado);

            var transacao = new Transacao(conta.Id, request.Valor, TipoTransacao.VendaCreditoParcelado, parcelas: request.Parcelas);

            await _contaRepository.AtualizarAsync(conta);

            await _transacaoRepository.AdicionarAsync(transacao);

            await _auditoriaService.RegistrarAsync("conta", conta.Id,
                    dados: $"Compra de R${transacao.Valor} registrada em {transacao.Parcelas} parcelas",
                    TipoTransacao.VendaCreditoParcelado,
                    StatusTransacao.Concluida);

            _logger.LogInformation($"Transação realizada com sucesso");
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
            _logger.LogError($"Erro ao realizar a transação", ex.Message);
            await _auditoriaService.RegistrarAsync("conta", request.ContaId,
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.VendaCreditoParcelado,
            StatusTransacao.Falhou);
        });
    }
}