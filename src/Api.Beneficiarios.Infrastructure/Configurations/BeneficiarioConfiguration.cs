using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Beneficiarios.Domain.Enums;


namespace Api.Beneficiarios.Infrastructure.Configurations;


/// <summary>
/// Configuração da entidade Beneficiario para o Entity Framework
/// Define mapeamento de tabela, colunas, índices e relacionamentos
/// </summary>
public class BeneficiarioConfiguration : IEntityTypeConfiguration<Beneficiario>
{
    public void Configure(EntityTypeBuilder<Beneficiario> builder)
    {

        // ===== Tabela =====
        builder.ToTable("Beneficiarios");

                // ===== Chaves primarias =====
                builder.HasKey(b => b.Id);

                // ========== PROPRIEDADES (COLUNAS) ==========

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
                    .HasColumnType("date") //Apenas data, sem hora
                    .IsRequired();


                builder.Property(b => b.Status)
                    .HasColumnName("Status")
                    .HasConversion<int>() // Armazenar enum como int
                    .IsRequired();

                builder.Property(b => b.PlanoId)
                    .HasColumnName("PlanoId")
                    .IsRequired();

                builder.Property(b => b.DataCadastro)
                    .HasColumnName("DataCadastro")
                    .HasColumnType("timestamp") //Data e hora
                    .IsRequired();

                builder.Property(b => b.DataAtualizacao)
                    .HasColumnName("DataAtualizacao")
                    .HasColumnType("timestamp") //Data e hora
                    .IsRequired();


                builder.Property(b => b.Excluido)
                    .HasColumnName("Excluido")
                    .HasDefaultValue(false) // Valor padrão no banco
                    .IsRequired();
                
                builder.Property(b => b.DataExclusao)
                    .HasColumnName("DataExclusao")
                    .HasColumnType("timestamp") //Data e hora
                    .IsRequired(false); // Pode ser nulo



                // ========== ÍNDICES ==========


                //Indice único no CPF
                builder.HasIndex(b => b.CPF)
                    .IsUnique()
                    .HasDatabaseName("IX_Beneficiarios_CPF");

                //Indice no PlanoId para otimizar consultas por plano
                builder.HasIndex(b => b.PlanoId)
                    .HasDatabaseName("IX_Beneficiarios_PlanoId"); 

                // Índice no Status (melhora performance de filtros)
                builder.HasIndex(b => b.Status)
                    .HasDatabaseName("IX_Beneficiarios_Status");    

                // Índice composto Status + PlanoId (para filtros combinados)
                builder.HasIndex(b => new {b.Status, b.PlanoId})
                    .HasDatabaseName("IX_Beneficiarios_Status_PlanoId");


                // Índice no Excluido (para query filter de soft delete)
                builder.HasIndex(b => b.Excluido)
                    .HasDatabaseName("IX_Beneficiarios_Excluido");


                // ========== RELACIONAMENTOS ==========

                //Relacionamento com plano (FK PlanoId) 1:N
                builder.HasOne(b => b.Plano)
                    .WithMany(p => p.Beneficiarios)
                    .HasForeignKey(b => b.PlanoId) // Chave estrangeira
                    .OnDelete(DeleteBehavior.Restrict) // Impede exclusão em cascata
                    .HasConstraintName("FK_Beneficiarios_Planos");
    }
}