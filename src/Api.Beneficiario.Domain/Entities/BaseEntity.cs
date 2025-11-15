namespace Api.Beneficiario.Domain.Entities;


public abstract class BaseEntity
{

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        DataAtualizacao = DateTime.UtcNow;
    }

    public Guid Id {get; set;}

    public DateTime DataCadastro {get; set;} = DateTime.UtcNow;

    public DateTime DataAtualizacao {get; set;}

    public bool Excluido {get; set;} = false;
    public DateTime? DataExclusao {get; set;}


}