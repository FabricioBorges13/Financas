
public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly AppDbContext _dbContext;

    public AuditoriaRepository(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }
    public async Task RegistrarAsync(Auditoria auditoria)
    {
        await _dbContext.Auditorias.AddAsync(auditoria);
        await _dbContext.SaveChangesAsync();
    }
}