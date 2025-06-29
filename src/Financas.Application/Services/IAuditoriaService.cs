public interface IAuditoriaService
{
    Task RegistrarAsync(string entidade, string dados, TipoTransacao tipoTransacao, StatusTransacao statusTransacao);

}