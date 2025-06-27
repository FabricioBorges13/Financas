using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly IRegistrarVendaUseCase _registrarVendaUseCase;

    public TransacoesController(IRegistrarVendaUseCase registrarVendaUseCase)
    {
        _registrarVendaUseCase = registrarVendaUseCase;
    }

    [HttpPost("venda")]
    public async Task<IActionResult> RegistrarVenda([FromBody] RegistrarVendaRequest request)
    {
        try
        {
            var resultado = await _registrarVendaUseCase.ExecutarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.TransacaoId }, resultado);
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
    public IActionResult ObterPorId(Guid id)
    {
        return Ok(new { id, mensagem = "Simulação de retorno. Implemente se desejar." });
    }
}
