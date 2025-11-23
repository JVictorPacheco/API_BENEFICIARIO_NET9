using System.ComponentModel.DataAnnotations;

namespace Api.Beneficiarios.Application.DTOs.Beneficiario;


/// <summary>
/// DTO para atualização de um beneficiário existente.
/// Permite atualizar nome, data de nascimento, status e plano.
/// CPF não pode ser alterado (identificador único do beneficiário).
/// </summary>
public class UpdateBeneficiarioDTO
{
    [Required(ErrorMessage = "Nome completo é obrigatório")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 150 caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Data de nascimento do beneficiário
    /// </summary>
    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime DataNascimento { get; set; }

    /// <summary>
    /// Status do beneficiário (Ativo ou Inativo)
    /// </summary>
    [Required(ErrorMessage = "Status é obrigatório")]
    public string Status { get; set; } = "Ativo";

    /// <summary>
    /// ID do plano ao qual o beneficiário está vinculado
    /// </summary>
    [Required(ErrorMessage = "PlanoId é obrigatório")]
    public Guid PlanoId { get; set; }


}