using Microsoft.EntityFrameworkCore;

public class ContaRepository : IContaRepository
{
    private readonly AppDbContext _dbContext;

    public ContaRepository(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public async Task AdicionarAsync(Conta conta)
    {
        await _dbContext.Contas.AddAsync(conta);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Conta conta)
    {
        _dbContext.Contas.Update(conta);
    }

    public async Task<List<ContaDTO>> BuscarContasAsync()
    {
        return await _dbContext.Contas
            .AsNoTracking()
            .Select(conta => new ContaDTO
            {
                DataAbertura = conta.DataAbertura,
                DataEncerramento = conta.DataEncerramento,
                Id = conta.Id,
                NumeroConta = conta.NumeroConta,
                SaldoBloqueado = conta.SaldoBloqueado,
                SaldoDisponivel = conta.SaldoDisponivel,
                SaldoFuturo = conta.SaldoFuturo,
                Status = conta.Status,
                TipoConta = conta.TipoConta
            })
            .ToListAsync();
    }

    public async Task<Conta?> BuscarPorNumeroConta(long numeroConta)
    {
        return await _dbContext.Contas.FirstOrDefaultAsync(x => x.NumeroConta == numeroConta);
    }

    public async Task<bool> ExisteNumeroContaAsync(long numeroConta)
    {
        return await _dbContext.Contas.AnyAsync(x => x.NumeroConta == numeroConta);
    }

    public async Task<Conta?> ObterPorIdAsync(Guid id)
    {
        return await _dbContext.Contas.FirstOrDefaultAsync(x => x.Id == id);
    }
}
