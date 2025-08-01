
using Microsoft.Extensions.Logging;

public class RegistrarVendaCreditoAVistaUseCase : IRegistrarVendaCreditoAVistaUseCase
{
    private readonly ILogger<RegistrarVendaCreditoAVistaUseCase> _logger;
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;

    public RegistrarVendaCreditoAVistaUseCase(ILogger<RegistrarVendaCreditoAVistaUseCase> logger, IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
        _logger = logger;
    }
    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaCreditoAvistaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Gerando lock para venda credito a vista");
        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.VendaCreditoAVista, request.ContaId, valor: request.Valor);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            _logger.LogInformation($"Buscando conta {request.ContaId} para realizar a transação");
            var conta = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (conta == null)
            {
                _logger.LogInformation($"Conta {request.ContaId} não encontrada!");
                throw new InvalidOperationException($"Conta {request.ContaId} não encontrada!");
            }

            _logger.LogInformation($"Registrando venda!");
            conta.RegistrarVenda(request.Valor, TipoTransacao.VendaCreditoAVista);

            var transacao = new Transacao(conta.Id, request.Valor, TipoTransacao.VendaCreditoAVista);

            await _contaRepository.AtualizarAsync(conta);

            await _transacaoRepository.AdicionarAsync(transacao);
            await _auditoriaService.RegistrarAsync("conta", conta.Id,
                    dados: $"Compra de R${transacao.Valor} registrada",
                    TipoTransacao.VendaCreditoAVista,
                    StatusTransacao.Concluida);

            _logger.LogInformation($"Transação realizada com sucesso");
            return new RegistrarVendaResponse
            {
                DataHora = DateTime.Now,
                SaldoFuturo = conta.SaldoFuturo,
                Tipo = TipoTransacao.VendaCreditoAVista,
                ContaId = conta.Id,
                TransacaoId = transacao.Id
            };
        }, cancellationToken,
        onFailure: async ex =>
        {
            _logger.LogError($"Erro ao realizar a transação", ex.Message);
            await _auditoriaService.RegistrarAsync("conta", request.ContaId,
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.VendaCreditoAVista,
            StatusTransacao.Falhou);
        });
    }
}