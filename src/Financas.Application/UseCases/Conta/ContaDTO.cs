public class ContaDTO
{
    public Guid Id { get; set; }
    public TipoConta TipoConta { get; set; }
    public StatusConta Status { get; set; }
    public long? NumeroConta { get; set; }
    public decimal? SaldoDisponivel { get; set; }
    public decimal? SaldoBloqueado { get; set; }
    public decimal? SaldoFuturo { get; set; }
    public DateTime? DataAbertura { get; set; }
    public DateTime? DataEncerramento { get; set; }
    public Cliente? Cliente { get; set; }
}