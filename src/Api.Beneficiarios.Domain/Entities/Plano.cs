using System.ComponentModel.DataAnnotations;


namespace Api.Beneficiarios.Domain.Entities;


/// <summary>
/// Representa um plano de saúde disponível para beneficiários
/// </summary>
public class Plano : BaseEntity
{

    /// <summary>
    /// Nome do plano (obrigatório, único)
    /// Exemplo: "Plano Ouro", "Plano Bronze", "Plano Prata"
    /// </summary>
    [Required(ErrorMessage = "Nome do plano é obrigarório")]
    [MaxLength(100, ErrorMessage = "Nome do plano deve ter no máximo 100 caracteres")]
    public string NomePlano { get; set; }


    /// <summary>
    /// Código de registro ANS - Agência Nacional de Saúde Suplementar (obrigatório, único)
    /// Exemplo: "ANS-123456", "ANS-100001"
    /// </summary>
    [Required (ErrorMessage = "Código de registro ANS é obrigatório")]
    [MaxLength(10, ErrorMessage = "Código de registro ANS deve ter no máximo 10 caracteres")]
    public string CodRegistroAns {get; set;}


    /// <summary>
    /// Indica se o plano está ativo para novas adesões
    /// Planos inativos não podem receber novos beneficiários
    /// </summary>
    public bool StatusPlano { get; set; } = true;

    /// <summary>
    /// Lista de beneficiários vinculados a este plano
    /// Relacionamento 1:N (um plano tem muitos beneficiários)
    /// </summary>
    public virtual ICollection<Beneficiario> Beneficiarios { get; set; } = new List<Beneficiario>(); // Inicializa a coleção para evitar null reference exceptions
}



