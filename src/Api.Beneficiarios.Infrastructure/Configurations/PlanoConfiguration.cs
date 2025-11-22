using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Beneficiario.Domain.Entities;

namespace Api.Beneficiario.Infrastructure.Configurations;

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

        builder.Property(p => p.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.CodigoRegistroANS)
            .HasColumnName("CodigoRegistroANS")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .HasColumnName("Ativo")
            .IsRequired()
            .HasDefaultValue(true); // Valor padrão no banco

        builder.Property(p => p.DataCadastro)
            .HasColumnName("DataCadastro")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(p => p.DataAtualizacao)
            .HasColumnName("DataAtualizacao")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(p => p.Excluido)
            .HasColumnName("Excluido")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.DataExclusao)
            .HasColumnName("DataExclusao")
            .HasColumnType("timestamp")
            .IsRequired(false); // Nullable

        // ========== ÍNDICES ==========

        // Índice único no Nome (regra: nome único no sistema)
        builder.HasIndex(p => p.Nome)
            .IsUnique()
            .HasDatabaseName("IX_Planos_Nome"); // Nome do índice no banco

        // Índice único no CodigoRegistroANS (regra: código ANS único)
        builder.HasIndex(p => p.CodigoRegistroANS) //
            .IsUnique()
            .HasDatabaseName("IX_Planos_CodigoRegistroANS");

        // Índice no Ativo (para filtros)
        builder.HasIndex(p => p.Ativo)
            .HasDatabaseName("IX_Planos_Ativo");

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