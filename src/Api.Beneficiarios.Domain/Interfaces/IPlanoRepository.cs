using Api.Beneficiarios.Domain.Entities;

namespace Api.Beneficiarios.Domain.Interfaces;

public interface IPlanoRepository
{
    Task<Plano> AdicionarPlanoAsync(Plano plano);
    
    Task<Plano?> ObterPlanoPorIdAsync(Guid id);
    
    Task<IEnumerable<Plano>> ObterTodosPlanosAsync();
    
    Task<Plano> AtualizarPlanoAsync(Plano plano);
    
    Task<bool> ExcluirPlanoSuavementeAsync(Guid id);
    
    Task<bool> ExistePlanoPorNomeAsync(string nomePlano);
    
    Task<bool> ExistePlanoPorCodAnsAsync(string codRegistroAns);
    
    Task<bool> PlanoTemBeneficiariosAsync(Guid id);
}
