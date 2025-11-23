using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Beneficiarios.Domain.Entities;

namespace Api.Beneficiarios.Infrastructure.Configurations;

public class PlanoConfiguration : IEntityTypeConfiguration<Plano>
{
    public void Configure(EntityTypeBuilder<Plano> builder)
    {
       
        
        builder.ToTable("Planos");

        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(p => p.NomePlano)
            .HasColumnName("NomePlano")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.CodRegistroAns)
            .HasColumnName("CodRegistroAns")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.StatusPlano)
            .HasColumnName("StatusPlano")
            .IsRequired()
            .HasDefaultValue(true); 

        builder.Property(p => p.DataCadastro)
            .HasColumnName("DataCadastro")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.DataAtualizacao)
            .HasColumnName("DataAtualizacao")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.Excluido)
            .HasColumnName("Excluido")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.DataExclusao)
            .HasColumnName("DataExclusao")
            .HasColumnType("timestamptz")
            .IsRequired(false); 

        
        builder.HasIndex(p => p.NomePlano)
            .IsUnique()
            .HasDatabaseName("IX_Planos_Nome"); 

        
        builder.HasIndex(p => p.CodRegistroAns) 
            .IsUnique()
            .HasDatabaseName("IX_Planos_CodRegistroANS");

        
        builder.HasIndex(p => p.StatusPlano)
            .HasDatabaseName("IX_Planos_StatusPlano");

        
        builder.HasIndex(p => p.Excluido)
            .HasDatabaseName("IX_Planos_Excluido");

        

        
        builder.HasMany(p => p.Beneficiarios) 
            .WithOne(b => b.Plano) 
            .HasForeignKey(b => b.PlanoId) 
            .OnDelete(DeleteBehavior.Restrict) 
            .HasConstraintName("FK_Beneficiarios_Planos"); 
    }
}