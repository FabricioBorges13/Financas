public class AdicionarSaldoRequest
{
    public Guid ContaId { get; set; }
    public long NumeroConta { get; set; }
    public decimal Saldo { get; set; }
}