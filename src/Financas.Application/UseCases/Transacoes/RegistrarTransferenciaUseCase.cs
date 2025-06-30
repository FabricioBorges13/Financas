
public class RegistrarTransferenciaUseCase : IRegistrarTransferenciaUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IResilienceService _resilienceService;

    public RegistrarTransferenciaUseCase(IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService, IResilienceService resilienceService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
        _resilienceService = resilienceService;
    }
    public async Task<RegistrarTransferenciaResponse> ExecutarAsync(RegistrarTransferenciaRequest request, CancellationToken cancellationToken)
    {
        var chaveLock = GeradorChave.GerarChaveLock(request.ContaId);
        var chaveTransacao = GeradorChave.GerarChaveIdempotencia(TipoTransacao.Transferencia, request.ContaId, contaDestinoId: request.ContaDestinoId, valor: request.Valor);

        return await _resilienceService.ExecuteAsync(chaveLock, chaveTransacao, async ct =>
        {
            var contaOrigem = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (contaOrigem == null)
                throw new InvalidOperationException("Conta de origem não encontrada!");

            var contaDestino = await _contaRepository.ObterPorIdAsync(request.ContaDestinoId);
            if (contaDestino == null)
                throw new InvalidOperationException("Conta de destino não encontrada!");

            contaOrigem.TransferirPara(contaDestino, request.Valor);

            var transacao = new Transacao(contaOrigem.Id, request.Valor, TipoTransacao.Transferencia, contaDestino.Id);
            await _contaRepository.AtualizarAsync(contaOrigem);
            await _contaRepository.AtualizarAsync(contaDestino);
            await _transacaoRepository.AdicionarAsync(transacao);

            await _auditoriaService.RegistrarAsync("conta",
                    dados: $"Transferência de R${transacao.Valor} registrada da conta origem {contaOrigem.Id} para conta destino {contaDestino.Id}",
                    TipoTransacao.Transferencia,
                    StatusTransacao.Concluida);

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
            await _auditoriaService.RegistrarAsync("conta",
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.Transferencia,
            StatusTransacao.Falhou);
        });
    }
}