public interface IAuditoriaRepository
{
    Task RegistrarAsync(Auditoria auditoria);    
    Task<List<Auditoria>> BuscarTodasAuditoriasAsync();
}