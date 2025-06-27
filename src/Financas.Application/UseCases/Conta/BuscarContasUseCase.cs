
public class BuscarContasUseCase : IBuscarContasUseCase
{
    private readonly IContaRepository _contaRepositor;
    public BuscarContasUseCase(IContaRepository contaRepository)
    {
        _contaRepositor = contaRepository;
    }
    public async Task<BuscarContasResponse> ExecutarAsync()
    {
        var contas = await  _contaRepositor.BuscarContasAsync();

        return new BuscarContasResponse
        {
            Contas = contas
        };        
    }
}