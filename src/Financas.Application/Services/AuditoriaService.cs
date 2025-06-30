
public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _repository;

    public AuditoriaService(IAuditoriaRepository repository)
    {
        _repository = repository;
    }
    public async Task RegistrarAsync(string entidade, Guid contaOrigemId, string dados, TipoTransacao tipoTransacao, StatusTransacao statusTransacao, Guid? contaDestinoId = null)
    {
        var auditoria = new Auditoria(tipoTransacao, contaOrigemId, entidade, dados, statusTransacao, contaDestinoId);
        await _repository.RegistrarAsync(auditoria);
    }
}