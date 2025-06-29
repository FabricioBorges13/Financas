public interface IAdicionarSaldoUseCase
{
    Task<ContaDTO> ExecutarAsync(AdicionarSaldoRequest request, CancellationToken cancellationToken);
}