using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuditoriaController : ControllerBase
{
    private readonly IBuscarTodasAuditoriasUseCase _buscarTodasAuditoriasUseCase;

    public AuditoriaController(IBuscarTodasAuditoriasUseCase buscarTodasAuditoriasUseCase)
    {
        _buscarTodasAuditoriasUseCase = buscarTodasAuditoriasUseCase;
    }


    [HttpGet()]
    public async Task<IActionResult> BuscarAuditorias()
    {
        try
        {
            return Ok(await _buscarTodasAuditoriasUseCase.ExecutarAsync());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

}
