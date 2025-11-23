using Api.Beneficiarios.Application.DTOs.Beneficiario;
using Api.Beneficiarios.Application.Services.Interfaces;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Enums;
using Api.Beneficiarios.Domain.Interfaces;


namespace Api.Beneficiarios.Application.Services;



public class BeneficiarioService : IBeneficiarioService
{
    private readonly IBeneficiarioRepository _beneficiarioRepository;

    public BeneficiarioService(IBeneficiarioRepository beneficiarioRepository)
    {
        _beneficiarioRepository = beneficiarioRepository;
    }


    public async Task<BeneficiarioResponseDto> CriarBeneficiariosAsync (CreateBeneficiarioDto dto)
    {

        var CpfExiste = await _beneficiarioRepository.ExisteBeneficiarioPorCPFAsync(dto.CPF);
        if(CpfExiste)
            throw new InvalidOperationException("CPF já cadastrado");

       
        var beneficiario = new Beneficiario
        {
            Nome = dto.Nome,
            CPF = dto.CPF,
            DataNascimento = dto.DataNascimento,
            PlanoId = dto.PlanoId,
            Status = StatusBeneficiario.Ativo
        };
       
        var resultado = await _beneficiarioRepository.AdicionarBeneficiarioAsync(beneficiario);

        return MapearParaDto(resultado);

    }

    public async Task<BeneficiarioResponseDto?> ObterBeneficiarioPorIdAsync(Guid id)
    {
        var beneficiario = await _beneficiarioRepository.ObterBeneficiarioPorIdAsync(id);
        
        if (beneficiario == null)
            return null;

        return MapearParaDto(beneficiario);
    }

    public async Task<IEnumerable<BeneficiarioResponseDto>> ObterTodosBeneficiariosAsync(string? status, Guid? planoId)
    {
        
        IEnumerable<Beneficiario> beneficiarios; 

       
        StatusBeneficiario? statusEnum = null;
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<StatusBeneficiario>(status, true, out var parsed))
                statusEnum = parsed;
        }

        beneficiarios = await _beneficiarioRepository.ObterBeneficiarioComFiltrosAsync(statusEnum, planoId);

        return beneficiarios.Select(MapearParaDto);
    }

    public async Task<BeneficiarioResponseDto?> AtualizarBeneficiarioAsync(Guid id, UpdateBeneficiarioDto dto)
    {
        
            var beneficiario = await _beneficiarioRepository.ObterBeneficiarioPorIdAsync(id);
        
        if (beneficiario == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Nome))
            beneficiario.Nome = dto.Nome;

        if (dto.DataNascimento.HasValue)
            beneficiario.DataNascimento = dto.DataNascimento.Value;

        if (dto.PlanoId.HasValue && dto.PlanoId.Value != Guid.Empty)
            beneficiario.PlanoId = dto.PlanoId.Value;

        if (!string.IsNullOrEmpty(dto.Status))
        {
            if (Enum.TryParse<StatusBeneficiario>(dto.Status, true, out var statusEnum))
                beneficiario.Status = statusEnum;
    }

        var resultado = await _beneficiarioRepository.AtualizarBeneficiarioAsync(beneficiario);

        return MapearParaDto(resultado);
    }

    public async Task<bool> ExcluirBeneficiarioSuavementeAsync(Guid id)
    {
        
        return await _beneficiarioRepository.ExcluirBeneficiarioSuavementeAsync(id);
    }
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
