
public class BuscarClienteUseCase : IBuscarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    public BuscarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }
    public async Task<ClienteDTO> ExecutarAsync(Guid id)
    {
        var cliente = await _clienteRepository.BuscarClientePorIdAsync(id);

        if (cliente == null)
            throw new KeyNotFoundException("Cliente não encontrado");

        return new ClienteDTO
        {
            Conta = cliente.Conta,
            Documento = cliente.Documento,
            Id = cliente.Id,
            Nome = cliente.Nome,
            TipoDocumento = cliente.TipoDocumento
        };
    }
}