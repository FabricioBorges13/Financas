using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Conta> Contas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
           .HasOne(c => c.Conta)
           .WithOne(c => c.Cliente)
           .HasForeignKey<Conta>(c => c.ClienteId)
           .IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}
