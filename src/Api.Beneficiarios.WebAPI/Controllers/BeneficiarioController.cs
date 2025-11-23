
using Api.Beneficiarios.Application.DTOs.Beneficiario;
using Api.Beneficiarios.Application.DTOs.Common;
using Api.Beneficiarios.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Beneficiarios.WebAPI.Controllers;


// Define a rota base: /api/beneficiarios
[Route("api/[controller]")]

// Indica que é um Controller de API (não MVC com views)
[ApiController]

// A classe herda de ControllerBase (base para APIs)
public class BeneficiarioController : ControllerBase
{
    // Campo privado que guarda a referência do Service
    private readonly IBeneficiarioService _beneficiarioService;

    // CONSTRUTOR: recebe o Service via Injeção de Dependência
    public BeneficiarioController(IBeneficiarioService beneficiarioService)
    {
        _beneficiarioService = beneficiarioService;
    }

    // ========== POST - CRIAR BENEFICIÁRIO ==========
    // Rota: POST /api/beneficiarios
    // Retorna: 201 Created (sucesso) ou 409 Conflict (CPF duplicado)
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CreateBeneficiarioDto dto)
    {
        // try-catch para capturar erros do Service
        try
        {
            // Chama o Service para criar
            var resultado = await _beneficiarioService.CriarBeneficiariosAsync(dto);

            // Retorna 201 Created com a URL do novo recurso
            // CreatedAtAction gera o header "Location" com a URL
            return CreatedAtAction(
                nameof(ObterPorId),           // Nome do método para gerar a URL
                new { id = resultado.Id },    // Parâmetro da URL
                resultado                      // Corpo da resposta (o DTO criado)
            );
        }
        catch (InvalidOperationException ex)
        {
            // Se CPF já existe, retorna 409 Conflict
            return Conflict(new ErrorResponse
            {
                Error = "ConflictError",
                Message = ex.Message
            });
        }
    }

    // ========== GET - LISTAR TODOS (COM FILTROS) ==========
    // Rota: GET /api/beneficiarios
    // Rota: GET /api/beneficiarios?status=Ativo
    // Rota: GET /api/beneficiarios?planoId=xxx
    // Rota: GET /api/beneficiarios?status=Ativo&planoId=xxx
    [HttpGet]
    public async Task<IActionResult> ObterTodos(
        [FromQuery] string? status = null,    // Filtro opcional via query string
        [FromQuery] Guid? planoId = null      // Filtro opcional via query string
    )
    {
        // Chama o Service passando os filtros
        var resultado = await _beneficiarioService.ObterTodosBeneficiariosAsync(status, planoId);

        // Retorna 200 OK com a lista
        return Ok(resultado);
    }

    // ========== GET - BUSCAR POR ID ==========
    // Rota: GET /api/beneficiarios/{id}
    // Retorna: 200 OK (encontrou) ou 404 Not Found
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
    {
        // Chama o Service para buscar
        var resultado = await _beneficiarioService.ObterBeneficiarioPorIdAsync(id);

        // Se não encontrou, retorna 404
        if (resultado == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Beneficiário não encontrado"
            });
        }

        // Se encontrou, retorna 200 OK
        return Ok(resultado);
    }

    // ========== PUT - ATUALIZAR ==========
    // Rota: PUT /api/beneficiarios/{id}
    // Retorna: 200 OK (sucesso) ou 404 Not Found
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(
        [FromRoute] Guid id,              // ID vem da URL
        [FromBody] UpdateBeneficiarioDto dto  // Dados vêm do corpo
    )
    {
        // Chama o Service para atualizar
        var resultado = await _beneficiarioService.AtualizarBeneficiarioAsync(id, dto);

        // Se não encontrou, retorna 404
        if (resultado == null)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Beneficiário não encontrado"
            });
        }

        // Se atualizou, retorna 200 OK com os dados atualizados
        return Ok(resultado);
    }

    // ========== DELETE - EXCLUIR ==========
    // Rota: DELETE /api/beneficiarios/{id}
    // Retorna: 204 No Content (sucesso) ou 404 Not Found
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir([FromRoute] Guid id)
    {
        // Chama o Service para excluir (soft delete)
        var excluiu = await _beneficiarioService.ExcluirBeneficiarioSuavementeAsync(id);

        // Se não encontrou, retorna 404
        if (!excluiu)
        {
            return NotFound(new ErrorResponse
            {
                Error = "NotFound",
                Message = "Beneficiário não encontrado"
            });
        }

        // Se excluiu, retorna 204 No Content (sem corpo)
        return NoContent();
    }
}