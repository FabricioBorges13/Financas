public class RegistrarVendaResponse
{
    public Guid? TransacaoId { get; set; }
    public Guid? ContaId { get; set; }
    public decimal? SaldoDisponivel { get; set; }
    public decimal? SaldoFuturo { get; set; }
    public decimal? SaldoBloqueado { get; set; }
    public TipoTransacao? Tipo { get; set; }
    public int? NumeroParcelas { get; set; }
    public DateTime DataHora { get; set; }
}