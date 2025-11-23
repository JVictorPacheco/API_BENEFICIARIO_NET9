using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Enums;
using Api.Beneficiarios.Domain.Interfaces;
using Api.Beneficiarios.Infrastructure.Data;

namespace Api.Beneficiarios.Infrastructure.Repositories;



public class BeneficiarioRepository : IBeneficiarioRepository
{
    
    private readonly AppDbContext _context;

    
    public BeneficiarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Beneficiario> AdicionarBeneficiarioAsync(Beneficiario beneficiario)
    {
        await _context.Beneficiarios.AddAsync(beneficiario);
        await _context.SaveChangesAsync();
        return beneficiario;
    }

    
    public async Task<Beneficiario> AtualizarBeneficiarioAsync(Beneficiario beneficiario)
    {
        _context.Beneficiarios.Update(beneficiario);
        await _context.SaveChangesAsync();
        return beneficiario;
    }

    
    public async Task<bool> ExcluirBeneficiarioAsync(Guid id)
    {
        var beneficiario = await _context.Beneficiarios
            .IgnoreQueryFilters() 
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario == null)
            return false;

        _context.Beneficiarios.Remove(beneficiario);
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<bool> ExcluirBeneficiarioSuavementeAsync(Guid id)
    {
        var beneficiario = await _context.Beneficiarios
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario == null)
            return false;

        beneficiario.ExcluirSuavemente();
        
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<Beneficiario?> ObterBeneficiarioPorIdAsync(Guid id)
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano) 
            .FirstOrDefaultAsync(b => b.Id == id); 
        
        
    }

    
    public async Task<IEnumerable<Beneficiario>> ObterTodosBeneficiariosAsync()
    {
        return await _context.Beneficiarios 
            .Include(b => b.Plano) 
            .OrderBy(b => b.Nome) 
            .ToListAsync(); 
    }

        public async Task<IEnumerable<Beneficiario>> ObterBeneficiarioPorStatusAsync(StatusBeneficiario status) 
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano)
            .Where(b => b.Status == status)
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Beneficiario>> ObterBeneficiarioPorPlanoIdAsync(Guid planoId)
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano)
            .Where(b => b.PlanoId == planoId)
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Beneficiario>> ObterBeneficiarioComFiltrosAsync(StatusBeneficiario? status = null,
                                                                                                Guid? planoId = null)
    {
        var query = _context.Beneficiarios
            .Include(b => b.Plano) 
            .AsQueryable(); 

        
        if (status.HasValue) 
        {
            query = query.Where(b => b.Status == status.Value); 
        }

        if (planoId.HasValue)
        {
            query = query.Where(b => b.PlanoId == planoId.Value);
        }

        return await query
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }

    
    public async Task<bool> ExisteBeneficiarioPorCPFAsync(string cpf)
    {
        return await _context.Beneficiarios 
            .AnyAsync(b => b.CPF == cpf);
        
       
    }


    public async Task<bool> ExistePorCPFAsync(string cpf, Guid idExcluir)
    {
        return await _context.Beneficiarios
            
            .AnyAsync(b => b.CPF == cpf && b.Id != idExcluir); 
    }

   
    public async Task<Beneficiario?> ObterBeneficiarioPorCPFAsync(string cpf)
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano)
            .FirstOrDefaultAsync(b => b.CPF == cpf);
    }
}

