namespace Api.Beneficiarios.Application.DTOs.Common;

/// <summary>
/// Resposta padronizada para erros da API.
/// </summary>
public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail>? Details { get; set; }
}

/// <summary>
/// Detalhe de um erro específico.
/// </summary>
public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
}