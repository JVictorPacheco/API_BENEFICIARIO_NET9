using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Enums;
using Api.Beneficiarios.Domain.Interfaces;
using Api.Beneficiarios.Infrastructure.Data;

namespace Api.Beneficiarios.Infrastructure.Repositories;


/// <summary>
/// Implementação do repositório de Beneficiário
/// Responsável por toda persistência e consulta de dados
/// Observe o uso de async/await para operações assíncronas
/// Operações assincronas são importantes para escalabilidade e performance, a definição de async/await permite que o thread não fique bloqueado durante operações de I/O
/// O repositório utiliza o AppDbContext do Entity Framework Core
/// O repositorio serve como camada de abstração entre a aplicação e o banco de dados
/// </summary>
public class BeneficiarioRepository : IBeneficiarioRepository
{
    /// <summary>
    /// Contexto do banco de dados via Entity Framework
    /// private readonly AppDbContext _context;
    /// private readondly serve para garantir que o contexto não seja modificado após a injeção
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Construtor que recebe o contexto via injeção de dependência
    /// ele recebe o contexto do banco de dados via injeção de dependência
    /// injeção é o processo de fornecer as dependências necessárias para uma classe, neste caso o contexto do banco de dados
    /// </summary>
    /// <param name="context"></param>
    public BeneficiarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // ========== OPERAÇÕES BÁSICAS (CRUD) ==========

    /// <summary>
    /// Adiciona um novo beneficiário ao banco de dados
    /// </summary>
    public async Task<Beneficiario> AdicionarBeneficiarioAsync(Beneficiario beneficiario)
    {
        await _context.Beneficiarios.AddAsync(beneficiario);
        await _context.SaveChangesAsync();
        return beneficiario;
    }

    /// <summary>
    /// Atualiza os dados de um beneficiário existente
    /// </summary>
    public async Task<Beneficiario> AtualizarBeneficiarioAsync(Beneficiario beneficiario)
    {
        _context.Beneficiarios.Update(beneficiario);
        await _context.SaveChangesAsync();
        return beneficiario;
    }

    /// <summary>
    /// Remove um beneficiário do banco (hard delete)
    /// ATENÇÃO: Apaga permanentemente do banco!
    /// </summary>
    public async Task<bool> ExcluirBeneficiarioAsync(Guid id)
    {
        var beneficiario = await _context.Beneficiarios
            .IgnoreQueryFilters() // Precisa ignorar para pegar mesmo se excluído
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario == null)
            return false;

        _context.Beneficiarios.Remove(beneficiario);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Marca um beneficiário como excluído (soft delete)
    /// RECOMENDADO: Mantém os dados no banco para auditoria
    /// </summary>
    public async Task<bool> ExcluirBeneficiarioSuavementeAsync(Guid id)
    {
        var beneficiario = await _context.Beneficiarios
            .FirstOrDefaultAsync(b => b.Id == id);

        if (beneficiario == null)
            return false;

        // Usa o método da entidade para soft delete
        beneficiario.ExcluirSuavemente();
        
        await _context.SaveChangesAsync();
        return true;
    }

    // ========== CONSULTAS (QUERIES) ==========

    /// <summary>
    /// Obtém um beneficiário por ID (não retorna excluídos)
    /// Inclui o Plano relacionado (eager loading)
    /// retorna null se não encontrado
    /// 
    /// </summary>
    public async Task<Beneficiario?> ObterBeneficiarioPorIdAsync(Guid id)
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano) // Carrega o Plano junto. Carrega plano para evitar múltiplas consultas ao acessar b.Plano
            .FirstOrDefaultAsync(b => b.Id == id); // Nessa linha já aplica o filtro de excluídos b => b.Id == id signfica que está buscando o beneficiário com o ID especificado
        
        // Query Filter já ignora excluídos automaticamente!
    }

    /// <summary>
    /// Obtém todos os beneficiários (não retorna excluídos)
    /// Inclui o Plano relacionado
    /// </summary>
    public async Task<IEnumerable<Beneficiario>> ObterTodosBeneficiariosAsync()
    {
        return await _context.Beneficiarios // refere-se à tabela Beneficiarios no banco. O contexto do EF mapeia a entidade Beneficiario para a tabela Beneficiarios
            .Include(b => b.Plano) // Aqui tras o plano porque provavelmente será usado
            .OrderBy(b => b.Nome) // Ordena por nome, pois é mais amigável
            .ToListAsync(); // Executa a consulta e retorna a lista de beneficiários porque ToListAsync materializa a consulta
    }

    /// <summary>
    /// Obtém beneficiários filtrados por status
    /// Exemplo: GET /api/beneficiarios?status=ATIVO
    /// IEnumerable é usado para retornar uma coleção de beneficiários
    /// </summary>
    public async Task<IEnumerable<Beneficiario>> ObterBeneficiarioPorStatusAsync(StatusBeneficiario status) // StatusBeneficiario é o enum definido para o status do beneficiário
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano)
            .Where(b => b.Status == status)
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }

    /// <summary>
    /// Obtém beneficiários filtrados por plano
    /// Exemplo: GET /api/beneficiarios?plano_id=xxx
    /// </summary>
    public async Task<IEnumerable<Beneficiario>> ObterBeneficiarioPorPlanoIdAsync(Guid planoId)
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano)
            .Where(b => b.PlanoId == planoId)
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }

    /// <summary>
    /// Obtém beneficiários com filtros combinados (status + plano)
    /// Exemplo: GET /api/beneficiarios?status=ATIVO&plano_id=xxx
    /// Parâmetros opcionais (nullable)
    /// </summary>
    public async Task<IEnumerable<Beneficiario>> ObterBeneficiarioComFiltrosAsync(StatusBeneficiario? status = null,
                                                                                                Guid? planoId = null)
    {
        var query = _context.Beneficiarios
            .Include(b => b.Plano) // Inclui o plano relacionado
            .AsQueryable(); // Permite construir a query dinamicamente

        // Aplica filtros condicionalmente
        if (status.HasValue) // Verifica se o status foi fornecido
        {
            query = query.Where(b => b.Status == status.Value); // Aplica o filtro de status
        }

        if (planoId.HasValue)
        {
            query = query.Where(b => b.PlanoId == planoId.Value);
        }

        return await query
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }

    // ========== VALIDAÇÕES ==========. ****PAREI AQUI****

    /// <summary>
    /// Verifica se já existe um beneficiário com o CPF informado
    /// Retorna true se existir (para validar CPF único)
    /// Usado na criação de novo beneficiário
    /// </summary>
    public async Task<bool> ExisteBeneficiarioPorCPFAsync(string cpf)
    {
        return await _context.Beneficiarios // return await _context.Beneficiarios refere-se à tabela Beneficiarios no banco de dados
            .AnyAsync(b => b.CPF == cpf);
        
        // AnyAsync é mais rápido que Count ou FirstOrDefault
        // Retorna true/false direto
    }

    /// <summary>
    /// Verifica se já existe um beneficiário com o CPF, excluindo o ID informado
    /// Útil para validação em atualizações (não pode ter outro com mesmo CPF)
    /// </summary>
    public async Task<bool> ExistePorCPFAsync(string cpf, Guid idExcluir)
    {
        return await _context.Beneficiarios
            // Verifica se existe outro com mesmo CPF, excluindo o próprio ID. AnyAsync retorna true se encontrar algum registro que 
            // atenda à condição especificada no predicado fornecido (neste caso, b => b.CPF == cpf && b.Id != idExcluir).
            .AnyAsync(b => b.CPF == cpf && b.Id != idExcluir); 
    }

    /// <summary>
    /// Obtém um beneficiário pelo CPF
    /// Útil para buscar duplicados ou login
    /// </summary>
    public async Task<Beneficiario?> ObterBeneficiarioPorCPFAsync(string cpf)
    {
        return await _context.Beneficiarios
            .Include(b => b.Plano)
            .FirstOrDefaultAsync(b => b.CPF == cpf);
    }
}



// A diferença de .Where e .FirstOrDefaultAsync é que o Where retorna uma coleção (IQueryable) que pode ter zero ou mais elementos, 
//enquanto o FirstOrDefaultAsync retorna um único elemento (o primeiro que corresponde ao critério) ou null se nenhum for encontrado.