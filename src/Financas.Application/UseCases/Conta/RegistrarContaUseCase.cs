
public class RegistrarContaUseCase : IRegistrarContaUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly IClienteRepository _clienteRepository;
    public RegistrarContaUseCase(IContaRepository contaRepository, IClienteRepository clienteRepository)
    {
        _contaRepository = contaRepository;
        _clienteRepository = clienteRepository;
    }
    public async Task<RegistrarContaResponse> ExecutarAsync(RegistrarContaRequest request)
    {
        var clienteExistente = await _clienteRepository.BuscarClientePorIdAsync(request.ClienteId.GetValueOrDefault());
        if (clienteExistente == null)
            throw new InvalidOperationException("Cliente não cadastrado!");

        if (clienteExistente.Conta != null)
            throw new InvalidOperationException("Cliente já possuí conta cadastrada!");

        var novaConta = new Conta(request.TipoConta, clienteExistente);

        await GerarNumeroContaUnicoAsync(novaConta);

        await _contaRepository.AdicionarAsync(novaConta);

        clienteExistente.VincularConta(novaConta);

        await _clienteRepository.AtualizarAsync(clienteExistente);

        return new RegistrarContaResponse
        {
            Id = novaConta.Id,
            TipoConta = novaConta.TipoConta,
            NumeroConta = novaConta.NumeroConta,
            Status = novaConta.Status
        };
    }

    private async Task GerarNumeroContaUnicoAsync(Conta novaConta)
    {
        const int tentativasMax = 5;
        int tentativas = 0;

        while (tentativas < tentativasMax)
        {
            novaConta.GerarNumeroConta();

            bool existe = await _contaRepository.ExisteNumeroContaAsync(novaConta.NumeroConta);
            if (!existe)
                return;

            tentativas++;
        }

        throw new Exception($"Não foi possível gerar um número de conta único após {tentativasMax} tentativas.");
    }
}
