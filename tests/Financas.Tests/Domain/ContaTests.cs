using FluentAssertions;

public class ContaTests
{
    private Cliente _cliente;
    private Conta _conta;
    public ContaTests()
    {
        _cliente = new Cliente("teste", "51741608066", TipoDocumento.CPF);
        _conta = new Conta(TipoConta.Corrente, _cliente);
    }

    [Fact]
    public void AdicionarSaldo_DeveAumentarSaldoDisponivel()
    {
        // Arrange        
        var saldoAnterior = _conta.SaldoDisponivel;

        // Act
        _conta.AdicionarSaldo(100);

        // Assert
        _conta.SaldoDisponivel.Should().Be(saldoAnterior + 100);
    }

    [Fact]
    public void NaoDeveAdicionarSaldoNegativo()
    {
        Action act = () => _conta.AdicionarSaldo(-50);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Valor inválido para adicionar saldo.");
    }

    [Fact]
    public void RegistrarVendaDebito_DeveSerRealizada()
    {
        // Arrange    
        var valorAdicionar = 150;
        var valorVenda = 100;
        _conta.AdicionarSaldo(valorAdicionar);

        // Act
        _conta.RegistrarVenda(valorVenda, TipoTransacao.VendaDebito);

        // Assert
        _conta.SaldoDisponivel.Should().Be(valorAdicionar - valorVenda);
    }

    [Fact]
    public void NaoDeveRegistrarVendaDebito()
    {
        // Arrange    
        var valorAdicionar = 100;
        var valorVenda = 150;
        _conta.AdicionarSaldo(valorAdicionar);

        // Act
        Action act = () => _conta.RegistrarVenda(valorVenda, TipoTransacao.VendaDebito);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Saldo insuficiente.");
    }

    [Fact]
    public void RegistrarVendaCredito_DeveSerRealizada()
    {
        // Arrange    
        var valorVenda = 100;
        // Act
        _conta.RegistrarVenda(valorVenda, TipoTransacao.VendaCreditoAVista);

        // Assert
        _conta.SaldoFuturo.Should().Be(valorVenda);
    }

    [Fact]
    public void RegistrarVendaCreditoParcelado_DeveSerRealizada()
    {
        // Arrange    
        var valorVenda = 100;
        // Act
        _conta.RegistrarVenda(valorVenda, TipoTransacao.VendaCreditoParcelado);

        // Assert
        _conta.SaldoFuturo.Should().Be(valorVenda);
    }

    [Fact]
    public void Estorno_DeveSerRealizada()
    {
        // Arrange    
        var valorEstorno = 100;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);
        // Act
        _conta.EstornarTransacao(valorEstorno, TipoTransacao.VendaCreditoParcelado);

        // Assert
        _conta.SaldoDisponivel.Should().Be(saldo - valorEstorno);
    }

    [Fact]
    public void NãoDeveRealizar_Estorno_SaldoInsuficiente()
    {
        // Arrange    
        var valorEstorno = 200;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);

        // Act
        Action act = () => _conta.EstornarTransacao(valorEstorno, TipoTransacao.VendaDebito);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Saldo insuficiente para estorno.");
    }

    [Fact]
    public void NãoDeveRealizar_Estorno_TransacaoEstorno()
    {
        // Arrange    
        var valorEstorno = 100;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);

        // Act
        Action act = () => _conta.EstornarTransacao(valorEstorno, TipoTransacao.Estorno);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Não é possível estornar um estorno.");
    }

    [Fact]
    public void NãoDeveRealizar_Estorno_ContaInativa()
    {
        // Arrange    
        var valorEstorno = 100;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);
        _conta.Encerrar();

        // Act
        Action act = () => _conta.EstornarTransacao(valorEstorno, TipoTransacao.VendaDebito);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Conta inativa.");
    }

    [Fact]
    public void Transferencia_DeveSerRealizada()
    {
        // Arrange    
        var valor = 100;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        // Act
        _conta.TransferirPara(contaDestino, valor);

        // Assert
        contaDestino.SaldoDisponivel.Should().Be(valor);
    }

    [Fact]
    public void NãoDeveRealizar_Transferencia_ContaInativa()
    {
        // Arrange    
        var valor = 100;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        contaDestino.Encerrar();

        // Act
        Action act = () => _conta.TransferirPara(contaDestino, valor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Ambas as contas devem estar ativas.");
    }

    [Fact]
    public void NãoDeveRealizar_Transferencia_SaldoInsuficient()
    {
        // Arrange    
        var valor = 200;
        var saldo = 150;
        _conta.AdicionarSaldo(saldo);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        
        // Act
        Action act = () => _conta.TransferirPara(contaDestino, valor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Saldo insuficiente para transferência.");
    }
}