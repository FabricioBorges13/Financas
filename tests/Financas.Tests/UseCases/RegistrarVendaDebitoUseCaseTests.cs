using FluentAssertions;
using Moq;

public class RegistrarVendaDebitoUseCaseTests
{
    private Cliente _cliente;
    private Conta _conta;
    private Mock<IContaRepository> _contaRepository;
    private Mock<IAuditoriaService> _auditoriaService;
    private Mock<ITransacaoRepository> _transacaoRepository;
    private Mock<IResilienceService> _resilienceService;
    public RegistrarVendaDebitoUseCaseTests()
    {
        _cliente = new Cliente("teste", "51741608066", TipoDocumento.CPF);
        _conta = new Conta(TipoConta.Corrente, _cliente);
        _contaRepository = new Mock<IContaRepository>();
        _auditoriaService = new Mock<IAuditoriaService>();
        _transacaoRepository = new Mock<ITransacaoRepository>();
        _resilienceService = new Mock<IResilienceService>();
        _resilienceService
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<CancellationToken, Task<RegistrarVendaResponse>>>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Exception, Task>>()))
            .Returns((string lockKey, string idKey, Func<CancellationToken, Task<RegistrarVendaResponse>> action, CancellationToken ct, Func<Exception, Task> onFailure) => action(ct));

    }
    [Fact]
    public async void RegistrarVendadebito_DeveSerRegistrado()
    {
        //Arrange  
        var useCase = new RegistrarVendaDebitoUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);
        _conta.AdicionarSaldo(200);
        var request = new RegistrarVendaDebitoRequest
        {
            ContaId = _conta.Id,
            Valor = 200,
            NumeroConta = _conta.NumeroConta,
        };

        _contaRepository.Setup(x => x.BuscarPorNumeroConta(It.IsAny<long>())).ReturnsAsync(_conta);

        //Action
        var response = await useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        response.ContaId.Should().Be(_conta.Id);
        response.SaldoDisponivel.Should().Be(0);
        _contaRepository.Verify(x => x.AtualizarAsync(It.IsAny<Conta>()), Times.AtLeastOnce);
        _transacaoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Transacao>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarVendaDebito_DeveRetornarErroDeContaNaoEncontrada()
    {
        // Arrange
        var useCase = new RegistrarVendaDebitoUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);

        var request = new RegistrarVendaDebitoRequest
        {
            ContaId = _conta.Id,
            Valor = 200,
            NumeroConta = _conta.NumeroConta,
        };

        _contaRepository.Setup(x => x.BuscarPorNumeroConta(It.IsAny<long>()));

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Conta {_conta.NumeroConta} não encontrada!");
    }

    [Fact]
    public async Task RegistrarVendaDebito_DeveRetornarErroSaldoInsuficiente()
    {
        // Arrange
        var useCase = new RegistrarVendaDebitoUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);

        var request = new RegistrarVendaDebitoRequest
        {
            ContaId = _conta.Id,
            Valor = 200,
            NumeroConta = _conta.NumeroConta,
        };

        _contaRepository.Setup(x => x.BuscarPorNumeroConta(It.IsAny<long>())).ReturnsAsync(_conta);

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Saldo insuficiente.");
    }
}