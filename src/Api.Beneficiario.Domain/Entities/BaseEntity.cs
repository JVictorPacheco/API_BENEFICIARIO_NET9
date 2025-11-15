namespace Api.Beneficiario.Domain.Entities;


public abstract class BaseEntity
{
    public Guid Id {get; set;}

    public DateTime DataCriacao {get; set;}

    public DateTime DataAtualizacao {get; set;}

    
}