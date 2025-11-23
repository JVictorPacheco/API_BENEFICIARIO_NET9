using System.ComponentModel.DataAnnotations;


namespace Api.Beneficiarios.Application.DTOs.Beneficiario;


/// <summary>
/// DTO para criação de um novo beneficiário.
/// Contém apenas os campos necessários para cadastro.
/// </summary>
public class CreateBeneficiarioDTO 
{

    [Required(ErrorMessage = "Nome completo é obrigatório")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
    public string Nome { get; set; } = string.Empty; 


    [Required(ErrorMessage = "CPF é obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 dígitos")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números")]
    public string CPF { get; set; } = string.Empty;


    [Required(ErrorMessage = "Data de nascimento é obrigatóri")]
    public DateTime DataNascimento {get; set;}


    [Required(ErrorMessage = "Plano id é obrigatório")]
    public Guid PlanoId {get; set;} 
}