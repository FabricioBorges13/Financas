using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

public class ResilienceServiceTests
{
    private readonly Mock<AppDbContext> _dbContextMock;
    private readonly Mock<IRedisService> _cacheMock;
    private readonly ResilienceService _resilienceService;

    public ResilienceServiceTests()
    {
        _dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _cacheMock = new Mock<IRedisService>();
        _resilienceService = new ResilienceService(_dbContextMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_DeveExecutarComSucesso()
    {
        // Arrange
        var chaveLock = "lock-key";
        var chaveIdempotencia = "idem-key";
        var expectedResult = 42;

        _cacheMock.Setup(c => c.ExistsAsync(chaveIdempotencia)).ReturnsAsync(false);
        _cacheMock.Setup(c => c.SetIfNotExistsAsync(chaveLock, It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheMock.Setup(c => c.SetAsync(chaveIdempotencia, "executada", It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.ReleaseIfMatchAsync(chaveLock, It.IsAny<string>())).ReturnsAsync(true);

        var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(_dbContextMock.Object);
        var dbTransactionMock = new Mock<IDbContextTransaction>();

        // Setup para BeginTransactionAsync
        _dbContextMock.Setup(db => db.Database).Returns(databaseFacadeMock.Object);
        databaseFacadeMock
            .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbTransactionMock.Object);

        dbTransactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        dbTransactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var resilienceService = new ResilienceService(_dbContextMock.Object, _cacheMock.Object);

        // Act
        var result = await resilienceService.ExecuteAsync<int>(
            chaveLock,
            chaveIdempotencia,
            async ct =>
            {
                await Task.Delay(10, ct); // simula operação
                return expectedResult;
            },
            CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);

        _cacheMock.Verify(c => c.ExistsAsync(chaveIdempotencia), Times.Once);
        _cacheMock.Verify(c => c.SetIfNotExistsAsync(chaveLock, It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(chaveIdempotencia, "executada", It.IsAny<TimeSpan>()), Times.Once);
        _cacheMock.Verify(c => c.ReleaseIfMatchAsync(chaveLock, It.IsAny<string>()), Times.Once);

        dbTransactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        dbTransactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_FallbackChamadoQuandoExcecao()
    {
        // Arrange
        var chaveLock = "lock-key";
        var chaveIdempotencia = "idem-key";

        _cacheMock.Setup(c => c.ExistsAsync(chaveIdempotencia)).ReturnsAsync(false);
        _cacheMock.Setup(c => c.SetIfNotExistsAsync(chaveLock, It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(true);
        _cacheMock.Setup(c => c.ReleaseIfMatchAsync(chaveLock, It.IsAny<string>())).ReturnsAsync(true);

        var databaseFacadeMock = new Mock<DatabaseFacade>(_dbContextMock.Object);
        var dbTransactionMock = new Mock<IDbContextTransaction>();

        _dbContextMock.Setup(db => db.Database).Returns(databaseFacadeMock.Object);
        databaseFacadeMock
            .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbTransactionMock.Object);

        dbTransactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        dbTransactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        bool onFailureCalled = false;
        Func<Exception, Task> onFailure = ex =>
        {
            onFailureCalled = true;
            return Task.CompletedTask;
        };

        var resilienceService = new ResilienceService(_dbContextMock.Object, _cacheMock.Object);

        // Act
        var result = await resilienceService.ExecuteAsync<int>(
            chaveLock,
            chaveIdempotencia,
            async ct =>
            {
                await Task.Delay(10, ct);
                throw new InvalidOperationException("Erro esperado");
            },
            CancellationToken.None,
            onFailure);

        // Assert
        Assert.True(onFailureCalled);
        Assert.Equal(default(int), result);
    }
    
     [Fact]
    public async Task OperacoesConcorrentes_NaMesmaConta_DeveBloquearSegunda()
    {
        // Arrange
        var contaId = Guid.NewGuid();
        var chaveLock = GeradorChave.GerarChaveLock(contaId);
        var chaveIdempotencia = GeradorChave.GerarChaveIdempotencia(TipoTransacao.VendaCreditoAVista, contaId);

        _cacheMock.SetupSequence(x => x.SetIfNotExistsAsync(chaveLock, It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .ReturnsAsync(true)  // Primeira chamada: lock obtido
            .ReturnsAsync(false); // Segunda chamada: lock já em uso

        var resilienceService = new ResilienceService(_dbContextMock.Object, _cacheMock.Object);

        // Act
        var task1 = resilienceService.ExecuteAsync(chaveLock, chaveIdempotencia, async ct =>
        {
            await Task.Delay(100); // simula tempo de execução
            return true;
        }, CancellationToken.None);

        var task2 = resilienceService.ExecuteAsync(chaveLock, chaveIdempotencia, async ct =>
        {
            return true;
        }, CancellationToken.None);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => task2);
        var result1 = await task1;

        // Assert
        Assert.Equal("Recurso em uso. Tente novamente.", exception.Message);
    }

}
