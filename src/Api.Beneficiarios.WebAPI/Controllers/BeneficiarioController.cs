
using Api.Beneficiarios.Application.DTOs.Beneficiario;
using Api.Beneficiarios.Application.DTOs.Common;
using Api.Beneficiarios.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Beneficiarios.WebAPI.Controllers;



[Route("api/[controller]")]


[ApiController]


public class BeneficiarioController : ControllerBase
{
    
    private readonly IBeneficiarioService _beneficiarioService;

    
    public BeneficiarioController(IBeneficiarioService beneficiarioService)
    {
        _beneficiarioService = beneficiarioService;
    }

   
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateBeneficiarioDto dto)
    {
      
        try
        {
            
            var resultado = await _beneficiarioService.CriarBeneficiariosAsync(dto);

            
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
    public async Task<IActionResult> ObterTodos(
        [FromQuery] string? status = null,
        [FromQuery] Guid? planoId = null
    )
    {
        
        var resultado = await _beneficiarioService.ObterTodosBeneficiariosAsync(status, planoId);

        
        return Ok(resultado);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
    {
        
        var resultado = await _beneficiarioService.ObterBeneficiarioPorIdAsync(id);

        
        if (resultado == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Beneficiário não encontrado"
            });
        }

      
        return Ok(resultado);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(
        [FromRoute] Guid id,        
        [FromBody] UpdateBeneficiarioDto dto 
    )
    {
        
        var resultado = await _beneficiarioService.AtualizarBeneficiarioAsync(id, dto);

        
        if (resultado == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Beneficiário não encontrado"
            });
        }

        
        return Ok(resultado);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir([FromRoute] Guid id)
    {
        
        var excluiu = await _beneficiarioService.ExcluirBeneficiarioSuavementeAsync(id);

        
        if (!excluiu)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Beneficiário não encontrado"
            });
        }

        return NoContent();
    }
}