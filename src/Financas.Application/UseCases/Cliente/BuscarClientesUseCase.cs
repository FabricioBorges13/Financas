
public class BuscarClientesUseCase : IBuscarClientesUseCase
{
    private readonly IClienteRepository _clienteRepository;
    public BuscarClientesUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }
    public async Task<BuscarClientesResponse> ExecutarAsync()
    {
        var clientes = await  _clienteRepository.BuscarClientes();

        return new BuscarClientesResponse
        {
            Clientes = clientes
        };        
    }
}