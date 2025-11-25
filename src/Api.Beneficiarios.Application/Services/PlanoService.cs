using Api.Beneficiarios.Application.DTOs.Plano;
using Api.Beneficiarios.Application.Services.Interfaces;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Interfaces;

namespace Api.Beneficiarios.Application.Services;

public class PlanoService : IPlanoService
{
    private readonly IPlanoRepository _planoRepository;

    public PlanoService(IPlanoRepository planoRepository)
    {
        _planoRepository = planoRepository;
    }

    public async Task<PlanoResponseDto> CriarPlanoAsync(CreatePlanoDto dto)
    {
        var nomeExiste = await _planoRepository.ExistePlanoPorNomeAsync(dto.NomePlano);
        if (nomeExiste)
            throw new InvalidOperationException("Já existe um plano com este nome");

        var codAnsExiste = await _planoRepository.ExistePlanoPorCodAnsAsync(dto.CodRegistroAns);
        if (codAnsExiste)
            throw new InvalidOperationException("Já existe um plano com este código ANS");

        var plano = new Plano
        {
            NomePlano = dto.NomePlano,
            CodRegistroAns = dto.CodRegistroAns,
            StatusPlano = true
        };

        var resultado = await _planoRepository.AdicionarPlanoAsync(plano);

        return MapearParaDto(resultado);
    }

    public async Task<PlanoResponseDto?> ObterPlanoPorIdAsync(Guid id)
    {
        var plano = await _planoRepository.ObterPlanoPorIdAsync(id);

        if (plano == null)
            return null;

        return MapearParaDto(plano);
    }

    public async Task<IEnumerable<PlanoResponseDto>> ObterTodosPlanosAsync()
    {
        var planos = await _planoRepository.ObterTodosPlanosAsync();

        return planos.Select(MapearParaDto);
    }

    public async Task<PlanoResponseDto?> AtualizarPlanoAsync(Guid id, UpdatePlanoDto dto)
    {
        var plano = await _planoRepository.ObterPlanoPorIdAsync(id);

        if (plano == null)
            return null;

        if (!string.IsNullOrEmpty(dto.NomePlano))
            plano.NomePlano = dto.NomePlano;

        if (!string.IsNullOrEmpty(dto.CodRegistroAns))
            plano.CodRegistroAns = dto.CodRegistroAns;

        if (dto.StatusPlano.HasValue)
            plano.StatusPlano = dto.StatusPlano.Value;

        var resultado = await _planoRepository.AtualizarPlanoAsync(plano);

        return MapearParaDto(resultado);
    }

    public async Task<bool> ExcluirPlanoSuavementeAsync(Guid id)
    {
        var temBeneficiarios = await _planoRepository.PlanoTemBeneficiariosAsync(id);
        if (temBeneficiarios)
            throw new InvalidOperationException("Não é possível excluir um plano que possui beneficiários vinculados");

        return await _planoRepository.ExcluirPlanoSuavementeAsync(id);
    }

    private static PlanoResponseDto MapearParaDto(Plano plano)
    {
        return new PlanoResponseDto
        {
            Id = plano.Id,
            NomePlano = plano.NomePlano,
            CodRegistroAns = plano.CodRegistroAns,
            StatusPlano = plano.StatusPlano,
            DataCadastro = plano.DataCadastro,
            DataAtualizacao = plano.DataAtualizacao
        };
    }
}