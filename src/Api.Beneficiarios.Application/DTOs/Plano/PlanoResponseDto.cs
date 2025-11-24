namespace Api.Beneficiarios.Application.DTOs.Plano;

public class PlanoResponseDto
{
    public Guid Id { get; set; }
    public string NomePlano { get; set; } = string.Empty;
    public string CodRegistroAns { get; set; } = string.Empty;
    public bool StatusPlano { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataAtualizacao { get; set; }
}