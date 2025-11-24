using Api.Beneficiarios.Application.DTOs.Plano;

namespace Api.Beneficiarios.Application.Services.Interfaces;

public interface IPlanoService
{
    Task<PlanoResponseDto> CriarPlanoAsync(CreatePlanoDto dto);
    
    Task<PlanoResponseDto?> ObterPlanoPorIdAsync(Guid id);
    
    Task<IEnumerable<PlanoResponseDto>> ObterTodosPlanosAsync();
    
    Task<PlanoResponseDto?> AtualizarPlanoAsync(Guid id, UpdatePlanoDto dto);
    
    Task<bool> ExcluirPlanoSuavementeAsync(Guid id);
}