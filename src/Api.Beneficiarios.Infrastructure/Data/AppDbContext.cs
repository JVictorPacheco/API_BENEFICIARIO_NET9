using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Domain.Entities;

namespace Api.Beneficiarios.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbConextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Beneficiario> Beneficiarios { get; set; }


    public DbSet<Plano> Planos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Beneficiarios>().HasQueryFilter(b => !b.Excluido);

        modelBuilder.Entity<Plano>().HasQueryFilter(p => !p.Excluido);
    }


   // ========== SaveChanges Override (Atualiza DataAtualizacao automaticamente) ==========

    /// <summary>
    /// Sobrescreve SaveChanges para atualizar timestamps automaticamente
    /// </summary>
    public override int SaveChanges()
    {
        AtualizarTimestamps();
        return base.SaveChanges();
    }


    /// <summary>
    /// Sobrescreve SaveChangesAsync para atualizar timestamps automaticamente
    /// </summary>
    public override async Task<int> SavedChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }


    /// <summary>
    /// Atualiza automaticamente DataAtualizacao em todas as entidades modificadas
    /// </summary>
    public void AtualizarTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(entries => entries.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));


        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;


            if (entry.State == entityState.Added)
            {
                if (entity.DataCadastro == default)
                    entity.DataCadastro = DateTime.UtcNow;
            }

            // SEMPRE atualiza DataAtualizacao em qualquer mudança
            entity.DataAtualizacao = DateTime.UtcNow;

        }

    }

}