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
    }

    public async Task<List<Transacao>> BuscarTodasTrasacoesAsync()
    {
        return await _dbContext.Transacoes.ToListAsync();
    }

    public async Task<Transacao?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Transacoes.FirstOrDefaultAsync(t => t.Id == id);
    }
}
