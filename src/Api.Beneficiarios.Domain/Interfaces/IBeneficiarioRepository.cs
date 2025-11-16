using Api.Beneficiario.Domain.Entities;
using Api.Beneficiario.Domain.Enums;


namespace Api.Beneficiario.Domain.Interfaces;


public interface IBeneficiarioRepository
{
    Task<Beneficiario> AdicionaBeneficiarioAsync(Beneficiario beneficiario);

    Task<Beneficiario> AtualizaBeneficiarioAsync(Beneficiario beneficiario);

    Task<bool> RomoverBeneciarioAsync(Guid beneficiarioId);

    Task<bool> RemoverSuaveBeneficiarioAsync(Guid beneficiarioId);

    Task<Beneficiario> ObterBeneficiarioPorIdAsync(Guid beneficiarioId);

    Task<IEnumerable<Beneficiario>> ObterTodosBeneficiariosAsync();

    Task<IEnumerable<Beneficiario>> ObterBeneficiariosPorStatusAsync(StatusBeneficiario status);

    Task<IEnumerable<Beneficiario>> ObterBeneficiariosPorPlanoIdAsync(Guid planoId);
}