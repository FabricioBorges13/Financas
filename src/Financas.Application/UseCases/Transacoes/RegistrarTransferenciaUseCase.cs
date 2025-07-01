
using Microsoft.Extensions.Logging;

public class RegistrarTransferenciaUseCase : IRegistrarTransferenciaUseCase
{
    private readonly ILogger<RegistrarTransferenciaUseCase> _logger;
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;

    public RegistrarTransferenciaUseCase(ILogger<RegistrarTransferenciaUseCase> logger, IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
        _logger = logger;
    }
    public async Task<RegistrarTransferenciaResponse> ExecutarAsync(RegistrarTransferenciaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Gerando lock para transferência");
        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.Transferencia, request.ContaId, contaDestinoId: request.ContaDestinoId, valor: request.Valor);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            var contaOrigem = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (contaOrigem == null)
            {
                _logger.LogInformation($"Conta origem {request.ContaId} não encontrada!");
                throw new InvalidOperationException($"Conta origem {request.ContaId} não encontrada!");
            }

            var contaDestino = await _contaRepository.ObterPorIdAsync(request.ContaDestinoId);
            if (contaDestino == null)
            {
                _logger.LogInformation($"Conta destino {request.ContaDestinoId} não encontrada!");
                throw new InvalidOperationException($"Conta destino {request.ContaDestinoId} não encontrada!");
            }

            _logger.LogInformation($"Registrando transferência de {contaOrigem.Id} para {contaDestino.Id}!");
            contaOrigem.TransferirPara(contaDestino, request.Valor);

            var transacao = new Transacao(contaOrigem.Id, request.Valor, TipoTransacao.Transferencia, contaDestino.Id);
            await _contaRepository.AtualizarAsync(contaOrigem);
            await _contaRepository.AtualizarAsync(contaDestino);
            await _transacaoRepository.AdicionarAsync(transacao);

            await _auditoriaService.RegistrarAsync("conta", contaOrigem.Id,
                    dados: $"Transferência de R${transacao.Valor} registrada da conta origem {contaOrigem.Id} para conta destino {contaDestino.Id}",
                    TipoTransacao.Transferencia,
                    StatusTransacao.Concluida,
                    contaDestino.Id);

            _logger.LogInformation($"Transação realizada com sucesso");
            return new RegistrarTransferenciaResponse
            {
                ContaDestinoId = contaDestino.Id,
                ContaOrigem = contaOrigem.Id,
                Valor = transacao.Valor,
                DataHora = DateTime.Now
            };
        }, cancellationToken,
        onFailure: async ex =>
        {
            _logger.LogError($"Erro ao realizar a transação", ex.Message);
            await _auditoriaService.RegistrarAsync("conta", request.ContaId,
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.Transferencia,
            StatusTransacao.Falhou,
            request.ContaDestinoId);
        });
    }
}