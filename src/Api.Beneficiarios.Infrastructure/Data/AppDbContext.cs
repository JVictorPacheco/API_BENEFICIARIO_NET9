using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Domain.Entities;

namespace Api.Beneficiarios.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Beneficiario> Beneficiarios { get; set; }


    public DbSet<Plano> Planos { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Beneficiario>().HasQueryFilter(b => !b.Excluido);

        modelBuilder.Entity<Plano>().HasQueryFilter(p => !p.Excluido);
    }


    public override int SaveChanges()
    {
        AtualizarTimestamps();
        return base.SaveChanges();
    }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }


    
    private void AtualizarTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));


        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;


            if (entry.State == EntityState.Added)
            {
                if (entity.DataCadastro == default)
                    entity.DataCadastro = DateTime.UtcNow;
            }

            
            entity.DataAtualizacao = DateTime.UtcNow;

        }

    }

}