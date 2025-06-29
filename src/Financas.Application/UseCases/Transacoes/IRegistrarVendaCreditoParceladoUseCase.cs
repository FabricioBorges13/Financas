public interface IRegistrarVendaCreditoParceladoUseCase
{
    Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaCreditoParceladoRequest request);
}