public interface IRegistrarTransferenciaUseCase
{
    Task<RegistrarTransferenciaResponse> ExecutarAsync(RegistrarTransferenciaRequest request,  CancellationToken cancellationToken);

}