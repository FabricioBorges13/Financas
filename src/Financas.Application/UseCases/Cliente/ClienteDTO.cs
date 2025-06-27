public class ClienteDTO
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Documento { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public Conta? Conta { get; set; }
}