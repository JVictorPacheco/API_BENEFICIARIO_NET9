using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Beneficiarios.Domain.Enums;


namespace Api.Beneficiarios.Infrastructure.Configurations;

public class BeneficiarioConfiguration : IEntityTypeConfiguration<Beneficiario>
{
    public void Configure(EntityTypeBuilder<Beneficiario> builder)
    {

        
        builder.ToTable("Beneficiarios");
             
                builder.HasKey(b => b.Id);

                builder.Property(b => b.Id)
                    .HasColumnName("Id")
                    .IsRequired();

                builder.Property(b => b.Nome)
                    .HasColumnName("Nome")
                    .HasMaxLength(150)
                    .IsRequired();

                builder.Property(b => b.CPF)
                    .HasColumnName("CPF")
                    .HasMaxLength(11)
                    .IsRequired();

                builder.Property(b => b.DataNascimento)
                    .HasColumnName("DataNascimento")
                    .HasColumnType("date") 
                    .IsRequired();


                builder.Property(b => b.Status)
                    .HasColumnName("Status")
                    .HasConversion<int>() 
                    .IsRequired();

                builder.Property(b => b.PlanoId)
                    .HasColumnName("PlanoId")
                    .IsRequired();

                builder.Property(b => b.DataCadastro)
                    .HasColumnName("DataCadastro")
                    .HasColumnType("timestamptz") 
                    .IsRequired();

                builder.Property(b => b.DataAtualizacao)
                    .HasColumnName("DataAtualizacao")
                    .HasColumnType("timestamptz") 
                    .IsRequired();


                builder.Property(b => b.Excluido)
                    .HasColumnName("Excluido")
                    .HasDefaultValue(false) 
                    .IsRequired();

                builder.Property(b => b.DataExclusao)
                    .HasColumnName("DataExclusao")
                    .HasColumnType("timestamptz") 
                    .IsRequired(false); 



                builder.HasIndex(b => b.CPF)
                    .IsUnique()
                    .HasDatabaseName("IX_Beneficiarios_CPF");

                
                builder.HasIndex(b => b.PlanoId)
                    .HasDatabaseName("IX_Beneficiarios_PlanoId"); 

               
                builder.HasIndex(b => b.Status)
                    .HasDatabaseName("IX_Beneficiarios_Status");    

                
                builder.HasIndex(b => new {b.Status, b.PlanoId})
                    .HasDatabaseName("IX_Beneficiarios_Status_PlanoId");


                
                builder.HasIndex(b => b.Excluido)
                    .HasDatabaseName("IX_Beneficiarios_Excluido");


                builder.HasOne(b => b.Plano)
                    .WithMany(p => p.Beneficiarios)
                    .HasForeignKey(b => b.PlanoId) 
                    .OnDelete(DeleteBehavior.Restrict) 
                    .HasConstraintName("FK_Beneficiarios_Planos");
    }
}