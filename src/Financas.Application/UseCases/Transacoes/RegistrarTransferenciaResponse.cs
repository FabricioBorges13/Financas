public class RegistrarTransferenciaResponse
{
    public Guid? ContaDestinoId { get; set; }
    public Guid? ContaOrigem { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataHora { get; set; }
}