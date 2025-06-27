public interface IRegistrarVendaUseCase
{
    Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaRequest request);
}