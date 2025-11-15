using System.ComponentModel.DataAnnotations;
using Api.Beneficiario.Domain.Enums;


namespace Api.Beneficiario.Domain.Entities;


public class Beneficiario : BaseEntity
{
    [Required(ErrorMessage = "Nome Obrigatório")]
    public string NomeCompleto {get; set;}


    [Required(ErrorMessage = "CPF Obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 digitos")]
    public string CPF {get; set;}


    [Required(ErrorMessage = "Datas de Nascimento Obrigatório")]
    public DateTime DatasNascimento {get; set;}

    public StatusBeneficiario Status {get; set;} = StatusBeneficiario.Ativo;

    [Required(ErrorMessage = "PlanoId Obrigatório")]
    public Guid PlanoId {get; set;}



}