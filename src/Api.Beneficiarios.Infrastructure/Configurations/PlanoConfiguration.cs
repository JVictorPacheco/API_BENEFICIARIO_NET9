using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Beneficiarios.Domain.Entities;

namespace Api.Beneficiarios.Infrastructure.Configurations;

/// <summary>
/// Configuração da entidade Plano para o Entity Framework
/// Define mapeamento de tabela, colunas, índices e relacionamentos
/// </summary>
public class PlanoConfiguration : IEntityTypeConfiguration<Plano>
{
    public void Configure(EntityTypeBuilder<Plano> builder)
    {
        // ========== TABELA ==========
        
        builder.ToTable("Planos");

        // ========== CHAVE PRIMÁRIA ==========
        
        builder.HasKey(p => p.Id);

        // ========== PROPRIEDADES (COLUNAS) ==========

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
            .HasDefaultValue(true); // Valor padrão no banco

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
            .IsRequired(false); // Nullable

        // ========== ÍNDICES ==========

        // Índice único no Nome (regra: nome único no sistema)
        builder.HasIndex(p => p.NomePlano)
            .IsUnique()
            .HasDatabaseName("IX_Planos_Nome"); // Nome do índice no banco

        // Índice único no CodigoRegistroANS (regra: código ANS único)
        builder.HasIndex(p => p.CodRegistroAns) //
            .IsUnique()
            .HasDatabaseName("IX_Planos_CodRegistroANS");

        // Índice no StatusPlano (para filtros)
        builder.HasIndex(p => p.StatusPlano)
            .HasDatabaseName("IX_Planos_StatusPlano");

        // Índice no Excluido (para query filter de soft delete)
        builder.HasIndex(p => p.Excluido)
            .HasDatabaseName("IX_Planos_Excluido");

        // ========== RELACIONAMENTOS ==========

        // Relacionamento com Beneficiarios (1:N)
        builder.HasMany(p => p.Beneficiarios) // Coleção de Beneficiários
            .WithOne(b => b.Plano) // Navegação inversa
            .HasForeignKey(b => b.PlanoId) // Chave estrangeira em Beneficiario
            .OnDelete(DeleteBehavior.Restrict) // Não permite deletar Plano se tiver Beneficiários
            .HasConstraintName("FK_Beneficiarios_Planos"); // Nome da FK no banco
    }
}