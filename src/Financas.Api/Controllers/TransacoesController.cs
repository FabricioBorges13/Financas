using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly IRegistrarVendaCreditoParceladoUseCase _registrarVendaCreditoParceladoUseCase;
    private readonly IRegistrarVendaCreditoAVistaUseCase _registrarVendaCreditoAvistaUseCase;
    private readonly IRegistrarVendaDebitoUseCase _registrarVendaDebitoUseCase;
    private readonly IRegistrarEstornoUseCase _registrarEstornoUseCase;
    private readonly IRegistrarTransferenciaUseCase _registrarTransferenciaUseCase;
    private readonly IBuscarTodasAsTransacoesUseCase _buscarTodasAsTransacoesUseCase;

    public TransacoesController(IRegistrarVendaCreditoParceladoUseCase registrarVendaUseCase,
    IRegistrarVendaCreditoAVistaUseCase registrarVendaCreditoAVistaUseCase,
    IRegistrarVendaDebitoUseCase registrarVendaDebitoUseCase,
    IRegistrarEstornoUseCase registrarEstornoUseCase,
    IRegistrarTransferenciaUseCase registrarTransferenciaUseCase,
    IBuscarTodasAsTransacoesUseCase buscarTodasAsTransacoesUseCase)
    {
        _registrarVendaCreditoParceladoUseCase = registrarVendaUseCase;
        _registrarVendaCreditoAvistaUseCase = registrarVendaCreditoAVistaUseCase;
        _registrarVendaDebitoUseCase = registrarVendaDebitoUseCase;
        _registrarEstornoUseCase = registrarEstornoUseCase;
        _registrarTransferenciaUseCase = registrarTransferenciaUseCase;
        _buscarTodasAsTransacoesUseCase = buscarTodasAsTransacoesUseCase;
    }

    [HttpPost("vendas/credito-avista")]
    public async Task<IActionResult> RegistrarCreditoAVista([FromBody] RegistrarVendaCreditoAvistaRequest request)
    {
        try
        {
            return Ok(await _registrarVendaCreditoAvistaUseCase.ExecutarAsync(request, HttpContext.RequestAborted));
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

    [HttpPost("vendas/credito-parcelado")]
    public async Task<IActionResult> RegistrarCreditoParcelado([FromBody] RegistrarVendaCreditoParceladoRequest request)
    {
        try
        {
            return Ok(await _registrarVendaCreditoParceladoUseCase.ExecutarAsync(request, HttpContext.RequestAborted));
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

    [HttpPost("vendas/debito")]
    public async Task<IActionResult> RegistrarDebito([FromBody] RegistrarVendaDebitoRequest request)
    {
        try
        {
            return Ok(await _registrarVendaDebitoUseCase.ExecutarAsync(request, HttpContext.RequestAborted));
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

    [HttpPost("vendas/estorno")]
    public async Task<IActionResult> RegistrarEstorno([FromBody] RegistrarEstornoRequest request)
    {
        try
        {
            return Ok(await _registrarEstornoUseCase.ExecutarAsync(request, HttpContext.RequestAborted));
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

    [HttpPost("vendas/transferencia")]
    public async Task<IActionResult> RegistrarTransferencia([FromBody] RegistrarTransferenciaRequest request)
    {
        try
        {
            return Ok(await _registrarTransferenciaUseCase.ExecutarAsync(request, HttpContext.RequestAborted));
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
  
    [HttpGet()]
    public async Task<IActionResult> BuscarTransacoes()
    {
        try
        {
            return Ok(await _buscarTodasAsTransacoesUseCase.ExecutarAsync());

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
