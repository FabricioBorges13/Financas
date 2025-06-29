public interface IRegistrarVendaCreditoAVistaUseCase
{
    Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaCreditoAvistaRequest request,CancellationToken cancellationToken);
}