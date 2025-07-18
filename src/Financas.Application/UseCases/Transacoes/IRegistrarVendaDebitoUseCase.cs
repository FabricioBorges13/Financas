public interface IRegistrarVendaDebitoUseCase
{
    Task<RegistrarVendaResponse> ExecutarAsync(RegistrarVendaDebitoRequest request, CancellationToken cancellationToken);
}