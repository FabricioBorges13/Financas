public class RegistrarVendaRequest
{
    public Guid ContaId { get; set; }
    public long NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public string? Descricao { get; set; }
}