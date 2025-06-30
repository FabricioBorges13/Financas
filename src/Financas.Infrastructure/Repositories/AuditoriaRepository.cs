

using Microsoft.EntityFrameworkCore;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly AppDbContext _dbContext;

    public AuditoriaRepository(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public async Task<List<Auditoria>> BuscarTodasAuditoriasAsync()
    {
        return await _dbContext.Auditorias.ToListAsync();
    }

    public async Task RegistrarAsync(Auditoria auditoria)
    {
        await _dbContext.Auditorias.AddAsync(auditoria);
        await _dbContext.SaveChangesAsync();
    }
}