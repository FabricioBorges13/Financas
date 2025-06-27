public class RegistrarVendaUseCase : IRegistrarVendaUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;

    public RegistrarVendaUseCase(
        IContaRepository contaRepository,
        ITransacaoRepository transacaoRepository)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
    }

    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaRequest request)
    {
        if (request.Valor <= 0)
            throw new ArgumentException("Valor da transação inválido.");

        if (request.ChaveIdempotencia != Guid.Empty)
        {
            var existe = await _transacaoRepository
                .ExistePorChaveIdempotenciaAsync(request.ChaveIdempotencia);

            if (existe)
                throw new InvalidOperationException("Transação já processada (idempotência).");
        }

        var conta = await _contaRepository.ObterPorIdAsync(request.ContaId)
            ?? throw new KeyNotFoundException("Conta não encontrada.");

        conta.RegistrarVenda(request.Valor, request.Tipo);

        var transacao = new Transacao(
            contaOrigemId: request.ContaId,
            valor: request.Valor,
            tipo: request.Tipo,
            chaveIdempotencia: request.ChaveIdempotencia,
            descricao: request.Descricao
        );

        transacao.MarcarComoConcluida();

        await _transacaoRepository.AdicionarAsync(transacao);
        await _contaRepository.AtualizarAsync(conta);

        return new RegistrarVendaResponse
        {
            TransacaoId = transacao.Id,
            NovoSaldoDisponivel = conta.SaldoDisponivel,
            Status = transacao.Status
        };
    }
}