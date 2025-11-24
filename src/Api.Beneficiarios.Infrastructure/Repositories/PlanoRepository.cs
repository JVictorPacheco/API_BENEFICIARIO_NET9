using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Interfaces;
using Api.Beneficiarios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Beneficiarios.Infrastructure.Repositories;

public class PlanoRepository : IPlanoRepository
{
    private readonly AppDbContext _context;

    public PlanoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Plano> AdicionarPlanoAsync(Plano plano)
    {
        plano.DataCadastro = DateTime.UtcNow;
        plano.DataAtualizacao = DateTime.UtcNow;

        _context.Planos.Add(plano);
        await _context.SaveChangesAsync();

        return plano;
    }

    public async Task<Plano?> ObterPlanoPorIdAsync(Guid id)
    {
        return await _context.Planos
            .Where(p => !p.Excluido)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Plano>> ObterTodosPlanosAsync()
    {
        return await _context.Planos
            .Where(p => !p.Excluido)
            .OrderBy(p => p.NomePlano)
            .ToListAsync();
    }

    public async Task<Plano> AtualizarPlanoAsync(Plano plano)
    {
        plano.DataAtualizacao = DateTime.UtcNow;

        _context.Planos.Update(plano);
        await _context.SaveChangesAsync();

        return plano;
    }

    public async Task<bool> ExcluirPlanoSuavementeAsync(Guid id)
    {
        var plano = await _context.Planos.FindAsync(id);

        if (plano == null || plano.Excluido)
            return false;

        plano.Excluido = true;
        plano.DataExclusao = DateTime.UtcNow;
        plano.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistePlanoPorNomeAsync(string nomePlano)
    {
        return await _context.Planos
            .Where(p => !p.Excluido)
            .AnyAsync(p => p.NomePlano == nomePlano);
    }

    public async Task<bool> ExistePlanoPorCodAnsAsync(string codRegistroAns)
    {
        return await _context.Planos
            .Where(p => !p.Excluido)
            .AnyAsync(p => p.CodRegistroAns == codRegistroAns);
    }

    public async Task<bool> PlanoTemBeneficiariosAsync(Guid id)
    {
        return await _context.Beneficiarios
            .Where(b => !b.Excluido)
            .AnyAsync(b => b.PlanoId == id);
    }
}