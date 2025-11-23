using Api.Beneficiarios.Application.DTOs.Beneficiario;
using Api.Beneficiarios.Application.Services.Interfaces;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Enums;
using Api.Beneficiarios.Domain.Interfaces;


namespace Api.Beneficiarios.Application;


// <summary>
/// Serviço responsável pela lógica de negócio de Beneficiário.
/// </summary>
public class BeneficiarioService : IBeneficiarioService
{
    private readonly IBeneficiarioRepository _beneficiarioRepository;

    public BeneficiarioService(IBeneficiarioRepository beneficiarioRepository)
    {
        _beneficiarioRepository = beneficiarioRepository;
    }


    public async Task<BeneficiarioResponseDto> CriarBeneficiariosAsync (CreateBeneficiarioDto dto)
    {
        // Verificar se o CPF existe
        var CpfExiste = await _beneficiarioRepository.ExisteBeneficiarioPorCPFAsync(dto.CPF);
        if(CpfExiste)
            throw new InvalidOperationException("CPF já cadastrado");

        // Cria a entidade a partir do DTO
        var beneficiario = new Beneficiario
        {
            Nome = dto.NomeCompleto,
            CPF = dto.CPF,
            DataNascimento = dto.DataNascimento,
            PlanoId = dto.PlanoId,
            Status = StatusBeneficiario.Ativo
        };

        // Salva no banco
        var resultado = await _beneficiarioRepository.AdicionarBeneficiarioAsync(beneficiario);

        // Retorna o DTO de resposta
        return MapearParaDto(resultado);

    }


    public async Task<BeneficiarioResponseDto?> ObterPorIdAsync(Guid id)
    {
        var beneficiario = await _beneficiarioRepository.ObterBeneficiarioPorIdAsync(id);
        
        if (beneficiario == null)
            return null;

        return MapearParaDto(beneficiario);
    }

    public async Task<IEnumerable<BeneficiarioResponseDto>> ObterTodosBeneficiariosAsync(string? status, Guid? planoId)
    {
        IEnumerable<Beneficiario> beneficiarios;

        // Converte string status para enum (se fornecido)
        StatusBeneficiario? statusEnum = null;
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<StatusBeneficiario>(status, true, out var parsed))
                statusEnum = parsed;
        }

        // Busca com filtros
        beneficiarios = await _beneficiarioRepository.ObterBeneficiarioComFiltrosAsync(statusEnum, planoId);

        // Converte para DTOs
        return beneficiarios.Select(MapearParaDto);
    }

    public async Task<BeneficiarioResponseDto?> AtualizarBeneficiarioAsync(Guid id, UpdateBeneficiarioDto dto)
    {
        // Busca o beneficiário
        var beneficiario = await _beneficiarioRepository.ObterBeneficiarioPorIdAsync(id);
        
        if (beneficiario == null)
            return null;

        // Atualiza os campos
        beneficiario.Nome = dto.NomeCompleto;
        beneficiario.DataNascimento = dto.DataNascimento;
        beneficiario.PlanoId = dto.PlanoId;

        // Atualiza o status
        if (Enum.TryParse<StatusBeneficiario>(dto.Status, true, out var statusEnum))
            beneficiario.Status = statusEnum;

        // Salva as alterações
        var resultado = await _beneficiarioRepository.AtualizarBeneficiarioAsync(beneficiario);

        return MapearParaDto(resultado);
    }

    public async Task<bool> ExcluirBeneficiarioSuavementeAsync(Guid id)
    {
        // Usa soft delete conforme implementado no repository
        return await _beneficiarioRepository.ExcluirBeneficiarioSuavementeAsync(id);
    }

    /// <summary>
    /// Converte uma entidade Beneficiario para BeneficiarioResponseDto.
    /// </summary>
    private static BeneficiarioResponseDto MapearParaDto(Beneficiario beneficiario)
    {
        return new BeneficiarioResponseDto
        {
            Id = beneficiario.Id,
            Nome = beneficiario.Nome,
            CPF = beneficiario.CPF,
            DataNascimento = beneficiario.DataNascimento,
            Status = beneficiario.Status.ToString(),
            PlanoId = beneficiario.PlanoId,
            NomePlano = beneficiario.Plano?.NomePlano ?? string.Empty,
            DataCadastro = beneficiario.DataCadastro,
            DataAtualizacao = beneficiario.DataAtualizacao
        };
    }
}
