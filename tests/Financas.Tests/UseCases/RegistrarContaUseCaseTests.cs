using FluentAssertions;
using Moq;

public class RegistrarContaUseCaseTests
{
    private Cliente _cliente;
    private Conta _conta;
    private Mock<IContaRepository> _contaRepository;
    private Mock<IClienteRepository> _clienteRepository;
    public RegistrarContaUseCaseTests()
    {
        _cliente = new Cliente("teste", "51741608066", TipoDocumento.CPF);
        _conta = new Conta(TipoConta.Corrente, _cliente);
        _contaRepository = new Mock<IContaRepository>();
        _clienteRepository = new Mock<IClienteRepository>();

    }
    [Fact]
    public async void RegistrarConta_DeveSerRegistrado()
    {
        //Arrange  
        var useCase = new RegistrarContaUseCase(_contaRepository.Object, _clienteRepository.Object);

        var request = new RegistrarContaRequest
        {
            ClienteId = _cliente.Id,
            TipoConta = TipoConta.Corrente
        };

        _clienteRepository.Setup(x => x.BuscarClientePorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_cliente);

        //Action
        var response = await useCase.ExecutarAsync(request);

        //Assert
        response.Id.Should().NotBeEmpty();
        response.Status.Should().Be(StatusConta.Ativa);
        response.NumeroConta.Should().BeGreaterThan(0);
        _contaRepository.Verify(x => x.AdicionarAsync(It.IsAny<Conta>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RegistrarConta_DeveRetornarErroDeClienteNaoEncontrada()
    {
        // Arrange
        var useCase = new RegistrarContaUseCase(_contaRepository.Object, _clienteRepository.Object);

        var request = new RegistrarContaRequest
        {
            ClienteId = _cliente.Id,
            TipoConta = TipoConta.Corrente
        };

        _clienteRepository.Setup(x => x.BuscarClientePorIdAsync(It.IsAny<Guid>()));

        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cliente não cadastrado!");
    }

    [Fact]
    public async Task RegistrarConta_DeveRetornarErroClienteJaPossuiConta()
    {
        // Arrange
        var useCase = new RegistrarContaUseCase(_contaRepository.Object, _clienteRepository.Object);
        _cliente.VincularConta(_conta);
        var request = new RegistrarContaRequest
        {
            ClienteId = _cliente.Id,
            TipoConta = TipoConta.Corrente
        };

        _clienteRepository.Setup(x => x.BuscarClientePorIdAsync(It.IsAny<Guid>())).ReturnsAsync(_cliente);
        
        //Action     
        Func<Task> act = () => useCase.ExecutarAsync(request);

        //Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cliente já possuí conta cadastrada!");
    }
}