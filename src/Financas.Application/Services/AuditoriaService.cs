
public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _repository;

    public AuditoriaService(IAuditoriaRepository repository)
    {
        _repository = repository;
    }
    public async Task RegistrarAsync(string entidade, string dados, TipoTransacao tipoTransacao, StatusTransacao statusTransacao)
    {
        var auditoria = new Auditoria(tipoTransacao, entidade, dados, statusTransacao);
        await _repository.RegistrarAsync(auditoria);
    }
}