public class RegistrarVendaRequest
{
    public Guid ContaId { get; set; }
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; } 
    public Guid ChaveIdempotencia { get; set; }
    public string? Descricao { get; set; }
}