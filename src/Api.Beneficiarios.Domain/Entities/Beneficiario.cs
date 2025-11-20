using System.ComponentModel.DataAnnotations;
using Api.Beneficiarios.Domain.Enums;


namespace Api.Beneficiarios.Domain.Entities;


public class Beneficiario : BaseEntity
{
    [Required(ErrorMessage = "Nome Obrigatório")]
    public string Nome {get; set;}


    [Required(ErrorMessage = "CPF Obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 digitos")]
    public string CPF {get; set;}


    [Required(ErrorMessage = "Datas de Nascimento Obrigatório")]
    public DateTime DataNascimento {get; set;}

    public StatusBeneficiario Status {get; set;} = StatusBeneficiario.Ativo;

    [Required(ErrorMessage = "PlanoId Obrigatório")]
    public Guid PlanoId {get; set;}

    public virtual Plano Plano {get; set;}


    // METODOS DE VALIDAÇÃO E ALTERAÇÃO DE STATUS


    public void Ativar()
    {
        if (Excluido)
            throw new InvalidOperationException("Não é possível ativar um beneficiário excluído.");

        Status = StatusBeneficiario.Ativo;
    } 


    public void Inativar()
    {
        Status = StatusBeneficiario.Inativo;
    }

    public void ExcluirSuavemente()
    {
        Excluido = true;
        DataExclusao = DateTime.UtcNow;
        Status = StatusBeneficiario.Inativo;
    }

    public bool PodeUsarPlano()
    {
        return Status == StatusBeneficiario.Ativo && !Excluido;
    }

    public bool CPFValido()
    {
        return !string.IsNullOrWhiteSpace(CPF) && CPF.Length == 11 && CPF.All(char.IsDigit);
    }

}