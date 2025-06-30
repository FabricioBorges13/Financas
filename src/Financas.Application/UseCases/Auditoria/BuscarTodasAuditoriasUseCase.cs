
public class BuscarTodasAuditoriasUseCase : IBuscarTodasAuditoriasUseCase
{
    private readonly IAuditoriaRepository _auditoriaRepository;
    public BuscarTodasAuditoriasUseCase(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }
    public async Task<BuscarTodasAuditoriasResponse> ExecutarAsync()
    {
        var auditorias = await _auditoriaRepository.BuscarTodasAuditoriasAsync();

        if (!auditorias.Any())
            throw new KeyNotFoundException("Não há auditorias disponíveis");

        return new BuscarTodasAuditoriasResponse
        {
           Auditorias = auditorias
        };
    }
}