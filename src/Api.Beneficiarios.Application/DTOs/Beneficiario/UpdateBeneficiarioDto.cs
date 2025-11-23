using System.ComponentModel.DataAnnotations;

namespace Api.Beneficiarios.Application.DTOs.Beneficiario;

public class UpdateBeneficiarioDto
{
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
    public string? Nome { get; set; } = string.Empty;

    public DateTime? DataNascimento { get; set; }

    public string? Status { get; set; } = "Ativo";

    public Guid? PlanoId { get; set; }


}