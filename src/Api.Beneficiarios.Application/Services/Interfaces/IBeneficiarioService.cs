using Api.Beneficiarios.Application.DTOs.Beneficiario;


namespace Api.Beneficiarios.Application.Services.Interfaces;

/// <summary>
/// Interface do serviço de Beneficiário.
/// Define as operações disponíveis para gerenciar beneficiários.
/// </summary>
public interface IBeneficiarioService
{
    Task<BeneficiarioResponseDto> CriarBeneficiaioAsync (CreateBeneficiarioDTO dto);

    Task<BeneficiarioResponseDto?> ObterBeneficiarioPorIdAsync (Guid Id); // 

    Task <IEnumerable<BeneficiarioResponseDto>> ObterTodosBeneficiariosAsync (string? status, Guid? planoId);

    Task<BeneficiarioResponseDto?> AtualizarAsync(Guid id, UpdateBeneficiarioDTO dto);

    Task<bool> ExcluirBeneficiarioAsync (Guid Id);
}