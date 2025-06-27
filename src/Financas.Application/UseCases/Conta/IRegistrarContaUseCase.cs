public interface IRegistrarContaUseCase
{
    Task<RegistrarContaResponse> ExecutarAsync(RegistrarContaRequest request);
}