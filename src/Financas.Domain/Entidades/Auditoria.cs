public class Auditoria
{
    public Guid Id { get; private set; }
    public DateTime DataHora { get; private set; }
    public TipoTransacao TipoTransacao { get; private set; }
    public string EntidadeAfetada { get; private set; }
    public string Dados { get; private set; }
    public StatusTransacao StatusTransacao { get; private set; }

    protected Auditoria() { }
    public Auditoria(TipoTransacao tipoTransacao, string entidade, string dados, StatusTransacao statusTransacao)
    {
        Id = Guid.NewGuid();
        DataHora = DateTime.Now;
        TipoTransacao = tipoTransacao;
        EntidadeAfetada = entidade;
        Dados = dados;
        StatusTransacao = statusTransacao;
    }
}