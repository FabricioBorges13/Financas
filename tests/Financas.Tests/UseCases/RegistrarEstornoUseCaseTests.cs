using FluentAssertions;
using Moq;

public class RegistrarEstornoUseCaseTests
{
    private Cliente _cliente;
    private Conta _conta;
    private Transacao _transacao;
    private Mock<IContaRepository> _contaRepository;
    private Mock<IAuditoriaService> _auditoriaService;
    private Mock<ITransacaoRepository> _transacaoRepository;
    private Mock<IResilienceService> _resilienceService;
    public RegistrarEstornoUseCaseTests()
    {
        _cliente = new Cliente("teste", "51741608066", TipoDocumento.CPF);
        _conta = new Conta(TipoConta.Corrente, _cliente);
        _transacao = new Transacao(_conta.Id, 100, TipoTransacao.VendaDebito);
        _contaRepository = new Mock<IContaRepository>();
        _auditoriaService = new Mock<IAuditoriaService>();
        _transacaoRepository = new Mock<ITransacaoRepository>();
        _resilienceService = new Mock<IResilienceService>();
        _resilienceService
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<CancellationToken, Task<RegistrarVendaResponse>>>(), It.IsAny<CancellationToken>(), It.IsAny<Func<Exception, Task>>()))
            .Returns((string lockKey, string idKey, Func<CancellationToken, Task<RegistrarVendaResponse>> action, CancellationToken ct, Func<Exception, Task> onFailure) => action(ct));

    }
    [Fact]
    public async void RegistrarEstorno_DeveSerRegistrado()
    {
        //Arrange  
        var useCase = new RegistrarEstornoUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);
        _conta.AdicionarSaldo(200);
        var request = new RegistrarEstornoRequest
        {
            ContaId = _conta.Id,
            TransacaoId = _transacao.Id
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_conta);
        _transacaoRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_transacao);

        //Action
        var response = await useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        response.ContaId.Should().Be(_conta.Id);
        response.SaldoDisponivel.Should().Be(100);
        _contaRepository.Verify(x => x.AtualizarAsync(It.IsAny<Conta>()), Times.AtLeastOnce);
        _transacaoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Transacao>()), Times.Once);
        _auditoriaService.Verify(x => x.RegistrarAsync(
            "conta",
            _conta.Id,
            It.Is<string>(msg => msg.Contains("Estorno")),
            TipoTransacao.Estorno,
            StatusTransacao.Concluida,
            It.Is<Guid?>(id => !id.HasValue)), Times.Once);
    }

    [Fact]
    public async Task RegistrarEstorno_DeveRetornarErroDeContaNaoEncontrada()
    {
        // Arrange
        var useCase = new RegistrarEstornoUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);

        var request = new RegistrarEstornoRequest
        {
            ContaId = _conta.Id,
            TransacaoId = _transacao.Id
        };

        _contaRepository.Setup(x => x.BuscarPorNumeroConta(It.IsAny<long>()));

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Conta {_conta.Id} não encontrada!");
    }

    [Fact]
    public async Task RegistrarEstorno_DeveRetornarErroSaldoInsuficiente()
    {
        // Arrange
        var useCase = new RegistrarEstornoUseCase(_contaRepository.Object, _transacaoRepository.Object, _auditoriaService.Object, _resilienceService.Object);

        var request = new RegistrarEstornoRequest
        {
            ContaId = _conta.Id,
            TransacaoId = _transacao.Id
        };

        _contaRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_conta);
        _transacaoRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_transacao);

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Saldo insuficiente para estorno.");
    }
}