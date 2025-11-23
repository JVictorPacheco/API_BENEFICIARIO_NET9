namespace Api.Beneficiarios.Application.DTOs.Beneficiario;


public class BeneficiarioResponseDto
{
    
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    
    public string CPF { get; set; } = string.Empty;
    
    public DateTime DataNascimento { get; set; }
    
    public string Status { get; set; } = string.Empty;
   
    public Guid PlanoId { get; set; }
    
    public string NomePlano { get; set; } = string.Empty;
    
    public DateTime DataCadastro { get; set; }
    
    public DateTime DataAtualizacao { get; set; }
}