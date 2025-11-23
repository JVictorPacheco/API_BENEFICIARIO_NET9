using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Enums;


namespace Api.Beneficiarios.Domain.Interfaces;


public interface IBeneficiarioRepository
{
    Task<Beneficiario> AdicionarBeneficiarioAsync(Beneficiario beneficiario);

    Task<Beneficiario> AtualizarBeneficiarioAsync(Beneficiario beneficiario);

    Task<bool> ExcluirBeneficiarioAsync(Guid beneficiarioId);

    Task<bool> ExcluirBeneficiarioSuavementeAsync(Guid beneficiarioId);

    Task<Beneficiario?> ObterBeneficiarioPorIdAsync (Guid beneficiarioId);

    Task<IEnumerable<Beneficiario>> ObterTodosBeneficiariosAsync();

    Task<IEnumerable<Beneficiario>> ObterBeneficiarioPorStatusAsync(StatusBeneficiario status);

    Task<IEnumerable<Beneficiario>> ObterBeneficiarioPorPlanoIdAsync(Guid planoId);


    Task<IEnumerable<Beneficiario>> ObterBeneficiarioComFiltrosAsync(
        StatusBeneficiario? status = null, 
        Guid? planoId = null
    );


    Task<bool> ExisteBeneficiarioPorCPFAsync(string cpf);
    
    // Task<bool> ExistePorCPFAsync(string cpf, Guid idExcluir); // Para update
    Task<Beneficiario?> ObterPorCPFAsync(string cpf);
}