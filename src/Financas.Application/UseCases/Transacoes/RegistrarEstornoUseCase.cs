
public class RegistrarEstornoUseCase : IRegistrarEstornoUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;

    public RegistrarEstornoUseCase(IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
    }
    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarEstornoRequest request)
    {
        try
        {
            Conta? contaDestino = null;
            var conta = await _contaRepository.ObterPorIdAsync(request.ContaId);
            if (conta == null)
                throw new InvalidOperationException($"Conta {request.ContaId} não encontrada!");

            var transacao = await _transacaoRepository.ObterPorIdAsync(request.TransacaoId);
            if (transacao == null)
                throw new InvalidOperationException($"Transação {request.TransacaoId} não encontrada!");

            if (transacao.Tipo == TipoTransacao.Transferencia)
            {
                contaDestino = await _contaRepository.ObterPorIdAsync(transacao.ContaDestinoId.GetValueOrDefault());
                if (contaDestino == null)
                    throw new InvalidOperationException($"Conta {transacao.ContaDestinoId} não encontrada!");

                contaDestino.AdicionarSaldo(transacao.Valor);
            }

            conta.EstornarTransacao(transacao.Valor, transacao.Tipo);
            var novaTransacao = new Transacao(conta.Id, transacao.Valor, TipoTransacao.Estorno);

            await _contaRepository.AtualizarAsync(conta);
            await _transacaoRepository.AdicionarAsync(novaTransacao);
            if (contaDestino != null)
                await _contaRepository.AtualizarAsync(contaDestino);
                
            await _auditoriaService.RegistrarAsync("conta",
                        dados: $"Estorno de R${transacao.Valor} registrada na conta {conta.Id}",
                        TipoTransacao.Estorno,
                        StatusTransacao.Concluida);

            return new RegistrarVendaResponse
            {
                ContaId = conta.Id,
                DataHora = DateTime.Now,
                SaldoBloqueado = conta.SaldoBloqueado,
                SaldoDisponivel = conta.SaldoDisponivel,
                SaldoFuturo = conta.SaldoFuturo,
                Tipo = TipoTransacao.Estorno,
                TransacaoId = transacao.Id
            };
        }
        catch (Exception ex)
        {
            await _auditoriaService.RegistrarAsync("conta",
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.Estorno,
            StatusTransacao.Falhou);

            throw new InvalidOperationException(ex.Message);
        }
    }
}