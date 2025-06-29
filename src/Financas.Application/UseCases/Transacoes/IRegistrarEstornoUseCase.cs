public interface IRegistrarEstornoUseCase
{
    Task<RegistrarVendaResponse> ExecutarAsync(RegistrarEstornoRequest request, CancellationToken cancellationToken);

}