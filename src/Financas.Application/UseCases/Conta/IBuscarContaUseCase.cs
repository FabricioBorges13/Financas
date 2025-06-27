public interface IBuscarContaUseCase
{
     Task<ContaDTO> ExecutarAsync(long numeroConta);
}