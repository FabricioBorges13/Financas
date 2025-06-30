
public class BuscarTodasAsTransacoesUseCase : IBuscarTodasAsTransacoesUseCase
{
    private readonly ITransacaoRepository _transacaoRepository;
    public BuscarTodasAsTransacoesUseCase(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }
    public async Task<BuscarTransacoesResponse> ExecutarAsync()
    {
        var transacoes = await _transacaoRepository.BuscarTodasTrasacoesAsync();

        if (!transacoes.Any())
            throw new KeyNotFoundException("Não há transações disponíveis");

        return new BuscarTransacoesResponse
        {
           Transacaos = transacoes
        };
    }
}