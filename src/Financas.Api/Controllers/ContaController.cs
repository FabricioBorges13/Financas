using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContaController : ControllerBase
{
    private readonly IRegistrarContaUseCase _registrarContaUseCase;
    private readonly IBuscarContaUseCase _buscarContaUseCase;
    private readonly IBuscarContasUseCase _buscarContasUseCase;

    public ContaController(IRegistrarContaUseCase registrarContaUseCase, IBuscarContaUseCase buscarContaUseCase, IBuscarContasUseCase buscarContasUseCase)
    {
        _registrarContaUseCase = registrarContaUseCase;
        _buscarContaUseCase = buscarContaUseCase;
        _buscarContasUseCase = buscarContasUseCase;
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

  
}
