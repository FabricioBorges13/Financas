using Microsoft.EntityFrameworkCore;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly AppDbContext _dbContext;

    public TransacaoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Transacao transacao)
    {
        await _dbContext.Transacoes.AddAsync(transacao);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<bool> ExistePorChaveIdempotenciaAsync(Guid chave)
    {
         return await _dbContext.Transacoes.AnyAsync(t => t.ChaveIdempotencia == chave);
    }

    public async Task<Transacao?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Transacoes.FirstOrDefaultAsync(t => t.Id == id);
    }
}
