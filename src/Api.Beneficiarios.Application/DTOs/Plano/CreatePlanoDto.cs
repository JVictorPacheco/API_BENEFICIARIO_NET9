using System.ComponentModel.DataAnnotations;

namespace Api.Beneficiarios.Application.DTOs.Plano;

public class CreatePlanoDto
{
    [Required(ErrorMessage = "Nome do plano é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string NomePlano { get; set; } = string.Empty;

    [Required(ErrorMessage = "Código de registro ANS é obrigatório")]
    [MaxLength(50, ErrorMessage = "Código ANS deve ter no máximo 50 caracteres")]
    public string CodRegistroAns { get; set; } = string.Empty;
}