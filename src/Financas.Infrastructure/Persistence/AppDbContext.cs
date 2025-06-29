using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Conta> Contas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
           .HasOne(c => c.Conta)
           .WithOne(c => c.Cliente)
           .HasForeignKey<Conta>(c => c.ClienteId)
           .IsRequired();

        modelBuilder.Entity<Conta>()
            .Property(c => c.Id)
            .HasColumnType("uniqueidentifier");

        modelBuilder.Entity<Cliente>()
            .Property(c => c.Id)
            .HasColumnType("uniqueidentifier");

        modelBuilder.Entity<Transacao>()
        .Property(c => c.Id)
        .HasColumnType("uniqueidentifier");

        modelBuilder.Entity<Conta>(entity =>
        {
            entity.Property(e => e.SaldoDisponivel).HasPrecision(18, 2);
            entity.Property(e => e.SaldoBloqueado).HasPrecision(18, 2);
            entity.Property(e => e.SaldoFuturo).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.Property(e => e.Valor).HasPrecision(18, 2);
        });

        base.OnModelCreating(modelBuilder);
    }
}
