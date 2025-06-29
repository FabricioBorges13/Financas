
public class RegistrarVendaDebitoUseCase : IRegistrarVendaDebitoUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IAuditoriaService _auditoriaService;

    public RegistrarVendaDebitoUseCase(IContaRepository contaRepository, ITransacaoRepository transacaoRepository, IAuditoriaService auditoriaService)
    {
        _contaRepository = contaRepository;
        _transacaoRepository = transacaoRepository;
        _auditoriaService = auditoriaService;
    }
    public async Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaDebitoRequest request)
    {
        try
        {
            var conta = await _contaRepository.BuscarPorNumeroConta(request.NumeroConta);
            if (conta == null)
                throw new InvalidOperationException($"Conta {request.NumeroConta} não encontrada!");

            conta.RegistrarVenda(request.Valor, TipoTransacao.VendaDebito);

            var transacao = new Transacao(conta.Id, request.Valor, TipoTransacao.VendaDebito);

            await _contaRepository.AtualizarAsync(conta);

            await _transacaoRepository.AdicionarAsync(transacao);
            await _auditoriaService.RegistrarAsync("conta",
                    dados: $"Compra de R${transacao.Valor} registrada",
                    TipoTransacao.VendaDebito,
                    StatusTransacao.Concluida);

            return new RegistrarVendaResponse
            {
                DataHora = DateTime.Now,
                SaldoFuturo = conta.SaldoFuturo,
                Tipo = TipoTransacao.VendaDebito,
                ContaId = conta.Id,
                TransacaoId = transacao.Id
            };
        }
        catch (Exception ex)
        {
            await _auditoriaService.RegistrarAsync("conta",
            dados: $"Falha ao realizar a transação: {ex.Message}",
            TipoTransacao.VendaDebito,
            StatusTransacao.Falhou);

            throw new InvalidOperationException(ex.Message);
        }
    }
}