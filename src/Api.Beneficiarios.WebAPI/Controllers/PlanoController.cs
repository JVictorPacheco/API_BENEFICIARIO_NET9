using Api.Beneficiarios.Application.DTOs.Common;
using Api.Beneficiarios.Application.DTOs.Plano;
using Api.Beneficiarios.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Beneficiarios.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlanoController : ControllerBase
{
    private readonly IPlanoService _planoService;

    public PlanoController(IPlanoService planoService)
    {
        _planoService = planoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreatePlanoDto dto)
    {
        try
        {
            var resultado = await _planoService.CriarPlanoAsync(dto);

            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = resultado.Id },
                resultado
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse
            {
                Error = "ConflictError",
                Message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var resultado = await _planoService.ObterTodosPlanosAsync();

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
    {
        var resultado = await _planoService.ObterPlanoPorIdAsync(id);

        if (resultado == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Plano não encontrado"
            });
        }

        return Ok(resultado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(
        [FromRoute] Guid id,
        [FromBody] UpdatePlanoDto dto
    )
    {
        var resultado = await _planoService.AtualizarPlanoAsync(id, dto);

        if (resultado == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Plano não encontrado"
            });
        }

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir([FromRoute] Guid id)
    {
        try
        {
            var excluiu = await _planoService.ExcluirPlanoSuavementeAsync(id);

            if (!excluiu)
            {
                return NotFound(new ErrorResponse
                {
                    Error = "NotFound",
                    Message = "Plano não encontrado"
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ErrorResponse
            {
                Error = "ConflictError",
                Message = ex.Message
            });
        }
    }
}