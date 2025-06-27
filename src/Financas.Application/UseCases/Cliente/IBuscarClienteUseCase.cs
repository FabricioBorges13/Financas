public interface IBuscarClienteUseCase
{
     Task<ClienteDTO> ExecutarAsync(Guid id);
}