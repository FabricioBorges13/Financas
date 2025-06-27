public class RegistrarVendaResponse
{
    public Guid TransacaoId { get; set; }
    public decimal NovoSaldoDisponivel { get; set; }
    public StatusTransacao Status { get; set; }
}