using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IRegistrarClienteUseCase _registrarClienteUseCase;
    private readonly IBuscarClientesUseCase _buscarClientesUseCase;
    private readonly IBuscarClienteUseCase _buscarClienteUseCase;

    public ClienteController(IRegistrarClienteUseCase registrarClienteUseCase, IBuscarClientesUseCase buscarClientesUseCase, IBuscarClienteUseCase buscarClienteUseCase)
    {
        _registrarClienteUseCase = registrarClienteUseCase;
        _buscarClientesUseCase = buscarClientesUseCase;
        _buscarClienteUseCase = buscarClienteUseCase;
    }

    [HttpPost("cliente")]
    public async Task<IActionResult> RegistrarCliente([FromBody] RegistrarClienteRequest request)
    {
        try
        {
            return Ok(await _registrarClienteUseCase.ExecutarAsync(request));
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        try
        {
            return Ok(await _buscarClienteUseCase.ExecutarAsync(id));

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet()]
    public async Task<IActionResult> BuscarClientes()
    {
        try
        {
            return Ok(await _buscarClientesUseCase.ExecutarAsync());

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
