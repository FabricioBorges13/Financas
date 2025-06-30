using FluentAssertions;
using Moq;

public class RegistrarTransferenciaUseCaseTests
{
    private Cliente _cliente;
    private Conta _conta;
    private Transacao _transacao;
    private Mock<IContaRepository> _contaRepository;
    private Mock<IAuditoriaService> _auditoriaService;
    private Mock<ITransacaoRepository> _transacaoRepository;
    private Mock<IResilienceService> _resilienceService;
    public RegistrarTransferenciaUseCaseTests()
    {
        _cliente = new Cliente("teste", "51741608066", TipoDocumento.CPF);
        _conta = new Conta(TipoConta.Corrente, _cliente);
        _transacao = new Transacao(_conta.Id, 100, TipoTransacao.VendaDebito);
        _contaRepository = new Mock<IContaRepository>();
        _auditoriaService = new Mock<IAuditoriaService>();
        _transacaoRepository = new Mock<ITransacaoRepository>();
        _resilienceService = new Mock<IResilienceService>();
        _resilienceService
    .Setup(x => x.ExecuteAsync(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Func<CancellationToken, Task<RegistrarTransferenciaResponse>>>(),
        It.IsAny<CancellationToken>(),
        It.IsAny<Func<Exception, Task>>()))
    .Returns<string, string, Func<CancellationToken, Task<RegistrarTransferenciaResponse>>, CancellationToken, Func<Exception, Task>>(
        async (lockKey, idempotencyKey, action, token, onFailure) =>
        {
            return await action(token);
        });
    }

    [Fact]
    public async void RegistrarTransferencia_DeveSerRegistrado()
    {
        //Arrange  
        var useCase = new RegistrarTransferenciaUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);
        _conta.AdicionarSaldo(200);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        var request = new RegistrarTransferenciaRequest
        {
            ContaId = _conta.Id,
            ContaDestinoId = contaDestino.Id,
            Valor = 100
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(_conta.Id)).ReturnsAsync(_conta);
        _contaRepository.Setup(x => x.ObterPorIdAsync(contaDestino.Id)).ReturnsAsync(contaDestino);
        //Action
        var response = await useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        response.ContaOrigem.Should().Be(_conta.Id);
        response.ContaDestinoId.Should().Be(contaDestino.Id);
        response.Valor.Should().Be(100);
        _contaRepository.Verify(x => x.AtualizarAsync(It.IsAny<Conta>()), Times.AtLeastOnce);
        _transacaoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Transacao>()), Times.Once);
        _auditoriaService.Verify(x => x.RegistrarAsync(
            "conta",
            It.Is<string>(msg => msg.Contains("Transferência")),
            TipoTransacao.Transferencia,
            StatusTransacao.Concluida), Times.Once);
    }

    [Fact]
    public async Task RegistrarTransferencia_DeveRetornarErroDeContaNaoEncontrada()
    {
        // Arrange
        var useCase = new RegistrarTransferenciaUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);

        var request = new RegistrarTransferenciaRequest
        {
            ContaId = _conta.Id,
            Valor = 100
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>()));

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Conta de origem não encontrada!");
    }

    [Fact]
    public async Task RegistrarTransferencia_DeveRetornarErroSaldoInsuficiente()
    {
        // Arrange
        var useCase = new RegistrarTransferenciaUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        var request = new RegistrarTransferenciaRequest
        {
            ContaId = _conta.Id,
            ContaDestinoId = contaDestino.Id,
            Valor = 100
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_conta);

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Saldo insuficiente para transferência.");
    }

    [Fact]
    public async Task RegistrarTransferencia_DeveRetornarErroDeContaDestinoNaoEncontrada()
    {
        // Arrange
        var useCase = new RegistrarTransferenciaUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        var request = new RegistrarTransferenciaRequest
        {
            ContaId = _conta.Id,
            Valor = 100
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(_conta.Id)).ReturnsAsync(_conta);
        _contaRepository.Setup(x => x.ObterPorIdAsync(contaDestino.Id));

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Conta de destino não encontrada!");
    }

    [Fact]
    public async Task RegistrarTransferencia_DeveRetornarErroContaInativa()
    {
        // Arrange
        var useCase = new RegistrarTransferenciaUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);
        var contaDestino = new Conta(TipoConta.Corrente, _cliente);
        contaDestino.Encerrar();
        var request = new RegistrarTransferenciaRequest
        {
            ContaId = _conta.Id,
            ContaDestinoId = contaDestino.Id,
            Valor = 100
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(_conta.Id)).ReturnsAsync(_conta);
        _contaRepository.Setup(x => x.ObterPorIdAsync(contaDestino.Id)).ReturnsAsync(contaDestino);
        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Ambas as contas devem estar ativas.");
    }
}