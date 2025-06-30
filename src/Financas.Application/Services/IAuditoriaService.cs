public interface IAuditoriaService
{
    Task RegistrarAsync(string entidade, Guid contaOrigemId, string dados, TipoTransacao tipoTransacao, StatusTransacao statusTransacao, Guid? contaDestinoId = null);

}