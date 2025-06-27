public interface IRegistrarClienteUseCase
{
    Task<RegistrarClienteResponse> ExecutarAsync(RegistrarClienteRequest request);
}