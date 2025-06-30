public interface IBuscarTodasAsTransacoesUseCase
{
    Task<BuscarTransacoesResponse> ExecutarAsync();
}