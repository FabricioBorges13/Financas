public interface IClienteRepository
{
    Task AdicionarAsync(Cliente cliente);
    Task<bool> ExisteClientePorDocumentoAsync(string documento);
    Task<Cliente?> BuscarClientePorIdAsync(Guid id);
    Task AtualizarAsync(Cliente cliente);
    Task<List<ClienteDTO>> BuscarClientes();
}