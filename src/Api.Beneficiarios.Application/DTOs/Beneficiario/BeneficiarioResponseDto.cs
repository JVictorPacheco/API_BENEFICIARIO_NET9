namespace Api.Beneficiarios.Application.DTOs.Beneficiario;

/// <summary>
/// DTO de resposta para beneficiário.
/// Contém apenas os dados que devem ser expostos ao cliente.
/// Note que não inclui campos internos como Excluido ou DataExclusao.
/// </summary>
public class BeneficiarioResponseDto
{
    /// <summary>
    /// Identificador único do beneficiário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome completo do beneficiário
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF do beneficiário (11 dígitos)
    /// </summary>
    public string CPF { get; set; } = string.Empty;

    /// <summary>
    /// Data de nascimento
    /// </summary>
    public DateTime DataNascimento { get; set; }

    /// <summary>
    /// Status atual (Ativo ou Inativo)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// ID do plano vinculado
    /// </summary>
    public Guid PlanoId { get; set; }

    /// <summary>
    /// Nome do plano vinculado
    /// </summary>
    public string NomePlano { get; set; } = string.Empty;

    /// <summary>
    /// Data de cadastro do beneficiário
    /// </summary>
    public DateTime DataCadastro { get; set; }

    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime DataAtualizacao { get; set; }
}