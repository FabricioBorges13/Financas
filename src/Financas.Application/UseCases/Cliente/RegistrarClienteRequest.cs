public class RegistrarClienteRequest
{
    public string? Nome { get; set; }
    public string? Documento { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
}