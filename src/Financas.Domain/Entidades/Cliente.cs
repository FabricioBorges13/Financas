public class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Documento { get; private set; }
    public TipoDocumento TipoDocumento { get; private set; }
    public Conta? Conta { get; private set; }

    protected Cliente() { }

    public Cliente(string nome, string documento, TipoDocumento tipoDocumento)
    {
        if (!ValidadorDocumento.EhValido(documento, tipoDocumento))
            throw new ArgumentException("Documento inválido para o tipo de pessoa.");

        Id = Guid.NewGuid();
        Nome = nome;
        Documento = documento;
        TipoDocumento = tipoDocumento;
    }

    public void VincularConta(Conta conta)
    {
        Conta = conta;
    }

}

