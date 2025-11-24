using Api.Beneficiarios.Application.DTOs.Plano;
using Api.Beneficiarios.Application.Services;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Api.Beneficiarios.Tests.Unit.Application.Services;

public class PlanoServiceTests
{
    private readonly Mock<IPlanoRepository> _planoRepositoryMock;
    private readonly PlanoService _planoService;

    public PlanoServiceTests()
    {
        _planoRepositoryMock = new Mock<IPlanoRepository>();
        _planoService = new PlanoService(_planoRepositoryMock.Object);
    }

    [Fact]
    public async Task CriarPlanoAsync_DeveRetornarPlano_QuandoDadosValidos()
    {
        // Arrange
        var dto = new CreatePlanoDto
        {
            NomePlano = "Plano Ouro",
            CodRegistroAns = "ANS-123456"
        };

        var planoEsperado = new Plano
        {
            Id = Guid.NewGuid(),
            NomePlano = dto.NomePlano,
            CodRegistroAns = dto.CodRegistroAns,
            StatusPlano = true,
            DataCadastro = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };

        _planoRepositoryMock
            .Setup(r => r.ExistePlanoPorNomeAsync(dto.NomePlano))
            .ReturnsAsync(false);

        _planoRepositoryMock
            .Setup(r => r.ExistePlanoPorCodAnsAsync(dto.CodRegistroAns))
            .ReturnsAsync(false);

        _planoRepositoryMock
            .Setup(r => r.AdicionarPlanoAsync(It.IsAny<Plano>()))
            .ReturnsAsync(planoEsperado);

        // Act
        var resultado = await _planoService.CriarPlanoAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NomePlano.Should().Be(dto.NomePlano);
        resultado.CodRegistroAns.Should().Be(dto.CodRegistroAns);
        resultado.StatusPlano.Should().BeTrue();
    }

    [Fact]
    public async Task CriarPlanoAsync_DeveLancarExcecao_QuandoNomeDuplicado()
    {
        // Arrange
        var dto = new CreatePlanoDto
        {
            NomePlano = "Plano Ouro",
            CodRegistroAns = "ANS-123456"
        };

        _planoRepositoryMock
            .Setup(r => r.ExistePlanoPorNomeAsync(dto.NomePlano))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _planoService.CriarPlanoAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe um plano com este nome");
    }

    [Fact]
    public async Task CriarPlanoAsync_DeveLancarExcecao_QuandoCodAnsDuplicado()
    {
        // Arrange
        var dto = new CreatePlanoDto
        {
            NomePlano = "Plano Ouro",
            CodRegistroAns = "ANS-123456"
        };

        _planoRepositoryMock
            .Setup(r => r.ExistePlanoPorNomeAsync(dto.NomePlano))
            .ReturnsAsync(false);

        _planoRepositoryMock
            .Setup(r => r.ExistePlanoPorCodAnsAsync(dto.CodRegistroAns))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _planoService.CriarPlanoAsync(dto);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe um plano com este código ANS");
    }

    [Fact]
    public async Task ObterPlanoPorIdAsync_DeveRetornarPlano_QuandoExiste()
    {
        // Arrange
        var planoId = Guid.NewGuid();
        var plano = new Plano
        {
            Id = planoId,
            NomePlano = "Plano Prata",
            CodRegistroAns = "ANS-654321",
            StatusPlano = true,
            DataCadastro = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };

        _planoRepositoryMock
            .Setup(r => r.ObterPlanoPorIdAsync(planoId))
            .ReturnsAsync(plano);

        // Act
        var resultado = await _planoService.ObterPlanoPorIdAsync(planoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(planoId);
        resultado.NomePlano.Should().Be("Plano Prata");
    }

    [Fact]
    public async Task ObterPlanoPorIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        // Arrange
        var planoId = Guid.NewGuid();

        _planoRepositoryMock
            .Setup(r => r.ObterPlanoPorIdAsync(planoId))
            .ReturnsAsync((Plano?)null);

        // Act
        var resultado = await _planoService.ObterPlanoPorIdAsync(planoId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodosPlanosAsync_DeveRetornarListaDePlanos()
    {
        // Arrange
        var planos = new List<Plano>
        {
            new Plano { Id = Guid.NewGuid(), NomePlano = "Plano Bronze", CodRegistroAns = "ANS-001", StatusPlano = true },
            new Plano { Id = Guid.NewGuid(), NomePlano = "Plano Prata", CodRegistroAns = "ANS-002", StatusPlano = true },
            new Plano { Id = Guid.NewGuid(), NomePlano = "Plano Ouro", CodRegistroAns = "ANS-003", StatusPlano = true }
        };

        _planoRepositoryMock
            .Setup(r => r.ObterTodosPlanosAsync())
            .ReturnsAsync(planos);

        // Act
        var resultado = await _planoService.ObterTodosPlanosAsync();

        // Assert
        resultado.Should().HaveCount(3);
    }

    [Fact]
    public async Task AtualizarPlanoAsync_DeveAtualizarPlano_QuandoExiste()
    {
        // Arrange
        var planoId = Guid.NewGuid();
        var planoExistente = new Plano
        {
            Id = planoId,
            NomePlano = "Plano Antigo",
            CodRegistroAns = "ANS-OLD",
            StatusPlano = true,
            DataCadastro = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };

        var dto = new UpdatePlanoDto
        {
            NomePlano = "Plano Novo",
            StatusPlano = false
        };

        _planoRepositoryMock
            .Setup(r => r.ObterPlanoPorIdAsync(planoId))
            .ReturnsAsync(planoExistente);

        _planoRepositoryMock
            .Setup(r => r.AtualizarPlanoAsync(It.IsAny<Plano>()))
            .ReturnsAsync((Plano p) => p);

        // Act
        var resultado = await _planoService.AtualizarPlanoAsync(planoId, dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.NomePlano.Should().Be("Plano Novo");
        resultado.StatusPlano.Should().BeFalse();
    }

    [Fact]
    public async Task AtualizarPlanoAsync_DeveRetornarNull_QuandoPlanoNaoExiste()
    {
        // Arrange
        var planoId = Guid.NewGuid();
        var dto = new UpdatePlanoDto { NomePlano = "Novo Nome" };

        _planoRepositoryMock
            .Setup(r => r.ObterPlanoPorIdAsync(planoId))
            .ReturnsAsync((Plano?)null);

        // Act
        var resultado = await _planoService.AtualizarPlanoAsync(planoId, dto);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ExcluirPlanoSuavementeAsync_DeveExcluir_QuandoNaoTemBeneficiarios()
    {
        // Arrange
        var planoId = Guid.NewGuid();

        _planoRepositoryMock
            .Setup(r => r.PlanoTemBeneficiariosAsync(planoId))
            .ReturnsAsync(false);

        _planoRepositoryMock
            .Setup(r => r.ExcluirPlanoSuavementeAsync(planoId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _planoService.ExcluirPlanoSuavementeAsync(planoId);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ExcluirPlanoSuavementeAsync_DeveLancarExcecao_QuandoTemBeneficiarios()
    {
        // Arrange
        var planoId = Guid.NewGuid();

        _planoRepositoryMock
            .Setup(r => r.PlanoTemBeneficiariosAsync(planoId))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _planoService.ExcluirPlanoSuavementeAsync(planoId);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Não é possível excluir um plano que possui beneficiários vinculados");
    }
}