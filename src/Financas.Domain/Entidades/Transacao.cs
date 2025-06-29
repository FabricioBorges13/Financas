public class Transacao
{
    public Guid Id { get; private set; }
    public Guid ContaOrigemId { get; private set; }
    public Guid? ContaDestinoId { get; private set; }
    public decimal Valor { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public StatusTransacao Status { get; private set; }
    public DateTime DataHora { get; private set; }
    public string? Descricao { get; private set; }
    public int? Parcelas { get; private set; }

    private Transacao() { }

    public Transacao(Guid contaOrigemId, decimal valor, TipoTransacao tipo, Guid? contaDestinoId = null, string? descricao = null, int? parcelas = null)
    {
        if (valor <= 0)
            throw new ArgumentException("O valor da transação deve ser maior que zero.");

        Id = Guid.NewGuid();
        ContaOrigemId = contaOrigemId;
        ContaDestinoId = contaDestinoId;
        Valor = valor;
        Tipo = tipo;
        Status = StatusTransacao.Pendente;
        DataHora = DateTime.UtcNow;
        Descricao = descricao;
        Parcelas = parcelas;
    }

    public void MarcarComoConcluida()
    {
        Status = StatusTransacao.Concluida;
    }

    public void MarcarComoFalha()
    {
        Status = StatusTransacao.Falhou;
    }  
}