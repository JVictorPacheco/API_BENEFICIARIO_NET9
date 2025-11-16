namespace Api.Beneficiarios.Domain.Entities;


public abstract class BaseEntity
{

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        DataCadastro = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
        
    }

    public Guid Id {get; set;}

    public DateTime DataCadastro {get; set;};

    public DateTime DataAtualizacao {get; set;}

    public bool Excluido {get; set;} = false;
    public DateTime? DataExclusao {get; set;}


}