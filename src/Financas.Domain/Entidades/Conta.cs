using System.Security.Cryptography;

public class Conta
{
    public Guid Id { get; private set; }
    public TipoConta TipoConta { get; private set; }
    public StatusConta Status { get; private set; }
    public long NumeroConta { get; private set; }
    public decimal SaldoDisponivel { get; private set; }
    public decimal SaldoBloqueado { get; private set; }
    public decimal SaldoFuturo { get; private set; }
    public DateTime DataAbertura { get; private set; }
    public DateTime? DataEncerramento { get; private set; }
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; }

    protected Conta() { }
    public Conta(TipoConta tipoConta, Cliente cliente)
    {
        Id = Guid.NewGuid();
        TipoConta = tipoConta;
        Status = StatusConta.Ativa;
        DataAbertura = DateTime.UtcNow;
        SaldoDisponivel = 0;
        SaldoBloqueado = 0;
        SaldoFuturo = 0;
        Cliente = cliente;
        ClienteId = cliente.Id;
    }

    public void Encerrar()
    {
        Status = StatusConta.Inativa;
        DataEncerramento = DateTime.UtcNow;
    }

    public void GerarNumeroConta()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[4];
            int numero;
            do
            {
                rng.GetBytes(bytes);
                numero = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            }
            while (numero < 100000 || numero > 999999);

            NumeroConta = numero;
        }
    }

    public void RegistrarVenda(decimal valor, TipoTransacao tipo)
    {
        if (Status != StatusConta.Ativa)
            throw new InvalidOperationException("A conta precisa estar ativa para registrar uma venda.");

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero.");

        switch (tipo)
        {
            case TipoTransacao.VendaDebito:
                if (SaldoDisponivel < valor)
                    throw new InvalidOperationException("Saldo insuficiente.");
                SaldoDisponivel -= valor;
                break;

            case TipoTransacao.VendaCreditoAVista:
            case TipoTransacao.VendaCreditoParcelado:
                SaldoFuturo += valor;
                break;

            default:
                throw new InvalidOperationException("Tipo de venda inválido.");
        }
    }

    public void Estornar(decimal valor)
    {
        if (Status != StatusConta.Ativa)
            throw new InvalidOperationException("A conta precisa estar ativa para estorno.");

        if (valor <= 0)
            throw new ArgumentException("Valor inválido.");

        SaldoDisponivel += valor;
    }

    public void TransferirPara(Conta destino, decimal valor)
    {
        if (Status != StatusConta.Ativa || destino.Status != StatusConta.Ativa)
            throw new InvalidOperationException("Ambas as contas devem estar ativas.");

        if (valor <= 0)
            throw new ArgumentException("Valor inválido.");

        if (SaldoDisponivel < valor)
            throw new InvalidOperationException("Saldo insuficiente para transferência.");

        SaldoDisponivel -= valor;
        destino.ReceberTransferencia(valor);
    }

    private void ReceberTransferencia(decimal valor)
    {
        SaldoDisponivel += valor;
    }
}