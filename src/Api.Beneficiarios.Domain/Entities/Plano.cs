using System.ComponentModel.DataAnnotations;


namespace Api.Beneficiarios.Domain.Entities;

public class Plano : BaseEntity
{

    [Required(ErrorMessage = "Nome do plano é obrigarório")]
    [MaxLength(100, ErrorMessage = "Nome do plano deve ter no máximo 100 caracteres")]
    public string NomePlano { get; set; }


    [Required (ErrorMessage = "Código de registro ANS é obrigatório")]
    [MaxLength(50, ErrorMessage = "Código de registro ANS deve ter no máximo 50 caracteres")]
    public string CodRegistroAns {get; set;}


    public bool StatusPlano { get; set; } = true;

    public virtual ICollection<Beneficiario> Beneficiarios { get; set; } = new List<Beneficiario>();
}



