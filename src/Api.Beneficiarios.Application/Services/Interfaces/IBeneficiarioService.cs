using Api.Beneficiarios.Application.DTOs.Beneficiario;


namespace Api.Beneficiarios.Application.Services.Interfaces;

/// <summary>
/// Interface do serviço de Beneficiário.
/// Define as operações disponíveis para gerenciar beneficiários.
/// </summary>
public interface IBeneficiarioService
{
    Task<BeneficiarioResponseDto> CriarBeneficiariosAsync(CreateBeneficiarioDto dto);

    Task<BeneficiarioResponseDto?> ObterBeneficiarioPorIdAsync(Guid id);

    Task<IEnumerable<BeneficiarioResponseDto>> ObterTodosBeneficiariosAsync(string? status, Guid? planoId);

    Task<BeneficiarioResponseDto?> AtualizarBeneficiarioAsync(Guid id, UpdateBeneficiarioDto dto);

    Task<bool> ExcluirBeneficiarioSuavementeAsync(Guid id);
}