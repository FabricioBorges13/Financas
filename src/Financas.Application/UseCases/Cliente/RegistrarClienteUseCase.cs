
public class RegistrarClienteUseCase : IRegistrarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    public RegistrarClienteUseCase(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }
    public async Task<RegistrarClienteResponse> ExecutarAsync(RegistrarClienteRequest request)
    {
        if(String.IsNullOrEmpty(request.Documento))
            throw new ArgumentException("Documento do cliente deve ser informado.");

        if (String.IsNullOrEmpty(request.Nome))
            throw new ArgumentException("Nome do cliente deve ser informado.");    

        var clienteExistente = await _clienteRepository.ExisteClientePorDocumentoAsync(request.Documento);
        if (clienteExistente)
            throw new ArgumentException("Cliente já cadastrado!");

        var novoCliente = new Cliente(request.Nome, request.Documento, request.TipoDocumento);
        await _clienteRepository.AdicionarAsync(novoCliente);

        return new RegistrarClienteResponse
        {
            Id = novoCliente.Id,
            Documento = novoCliente.Documento,
            Nome = novoCliente.Nome
        };
    }
}
