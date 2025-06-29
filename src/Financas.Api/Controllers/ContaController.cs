using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContaController : ControllerBase
{
    private readonly IRegistrarContaUseCase _registrarContaUseCase;
    private readonly IBuscarContaUseCase _buscarContaUseCase;
    private readonly IBuscarContasUseCase _buscarContasUseCase;
    private IAdicionarSaldoUseCase _adicionarSaldoUseCase;

    public ContaController(IRegistrarContaUseCase registrarContaUseCase, IBuscarContaUseCase buscarContaUseCase, IBuscarContasUseCase buscarContasUseCase, IAdicionarSaldoUseCase adicionarSaldoUseCase)
    {
        _registrarContaUseCase = registrarContaUseCase;
        _buscarContaUseCase = buscarContaUseCase;
        _buscarContasUseCase = buscarContasUseCase;
        _adicionarSaldoUseCase = adicionarSaldoUseCase;
    }

    [HttpPost("conta")]
    public async Task<IActionResult> RegistrarConta([FromBody] RegistrarContaRequest request)
    {
        try
        {
            return Ok(await _registrarContaUseCase.ExecutarAsync(request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{numero:long}")]
    public async Task<IActionResult> BuscarPorNumeroConta(long numero)
    {
        try
        {
            return Ok(await _buscarContaUseCase.ExecutarAsync(numero));

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet()]
    public async Task<IActionResult> BuscarContas()
    {
        try
        {
            return Ok(await _buscarContasUseCase.ExecutarAsync());

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("adicionar-saldo")]
    public async Task<IActionResult> AdicionarSaldo([FromBody] AdicionarSaldoRequest request)
    {
        try
        {
            return Ok(await _adicionarSaldoUseCase.ExecutarAsync(request, HttpContext.RequestAborted));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { mensagem = ex.Message });
        }
    }
}
