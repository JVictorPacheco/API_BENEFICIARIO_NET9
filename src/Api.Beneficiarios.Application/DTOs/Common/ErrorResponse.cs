namespace Api.Beneficiarios.Application.DTOs.Common;


public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail>? Details { get; set; }
}


public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
}