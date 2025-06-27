using Microsoft.EntityFrameworkCore;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _dbContext;

    public ClienteRepository(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        await _dbContext.Clientes.AddAsync(cliente);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        _dbContext.Clientes.Update(cliente);        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Cliente?> BuscarClientePorIdAsync(Guid id)
    {
        return await _dbContext.Clientes.Include(x => x.Conta).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ClienteDTO>> BuscarClientes()
    {
        return await _dbContext.Clientes
        .AsNoTracking()
        .Select(cliente => new ClienteDTO
        {
            Documento = cliente.Documento,
            Id = cliente.Id,
            Nome = cliente.Nome
        })
        .ToListAsync();
    }

    public async Task<bool> ExisteClientePorDocumentoAsync(string documento)
    {
        return await _dbContext.Clientes.AnyAsync(x => x.Documento == documento);
    }
}
