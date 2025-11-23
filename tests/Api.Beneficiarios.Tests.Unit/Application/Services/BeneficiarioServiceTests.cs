using Api.Beneficiarios.Application.DTOs.Beneficiario;
using Api.Beneficiarios.Application.Services;
using Api.Beneficiarios.Domain.Entities;
using Api.Beneficiarios.Domain.Enums;
using Api.Beneficiarios.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Api.Beneficiarios.Tests.Unit.Services;



public class BeneficiarioServiceTests
{

    private readonly Mock<IBeneficiarioRepository> _repositoryMock;


    private readonly BeneficiarioService _service;



    public BeneficiarioServiceTests()
    {

        _repositoryMock = new Mock<IBeneficiarioRepository>();


        _service = new BeneficiarioService(_repositoryMock.Object);
    }


    [Fact]
    public async Task CriarBeneficiarioAsync_QuandoCpfNaoExiste_DeveRetornarDto()
    {

        var dto = new CreateBeneficiarioDto
        {
            Nome = "João Victor",
            CPF = "12345678901",
            DataNascimento = new DateTime(1990, 5, 15),
            PlanoId = Guid.NewGuid()
        };


        _repositoryMock
            .Setup(r => r.ExisteBeneficiarioPorCPFAsync(dto.CPF))
            .ReturnsAsync(false);


        _repositoryMock
            .Setup(r => r.AdicionarBeneficiarioAsync(It.IsAny<Beneficiario>()))
            .ReturnsAsync((Beneficiario b) =>
            {

                b.Id = Guid.NewGuid();
                b.DataCadastro = DateTime.UtcNow;
                b.DataAtualizacao = DateTime.UtcNow;
                return b;
            });


        var resultado = await _service.CriarBeneficiariosAsync(dto);


        resultado.Should().NotBeNull();


        resultado.Nome.Should().Be(dto.Nome);
        resultado.CPF.Should().Be(dto.CPF);
        resultado.Status.Should().Be("Ativo");


        _repositoryMock.Verify(
            r => r.AdicionarBeneficiarioAsync(It.IsAny<Beneficiario>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CriarBeneficiarioAsync_QuandoCpfJaExiste_DeveLancarExcecao()
    {

        var dto = new CreateBeneficiarioDto
        {
            Nome = "Maria Silva",
            CPF = "12345678901",
            DataNascimento = new DateTime(1985, 3, 20),
            PlanoId = Guid.NewGuid()
        };


        _repositoryMock
            .Setup(r => r.ExisteBeneficiarioPorCPFAsync(dto.CPF))
            .ReturnsAsync(true);


        await FluentActions
            .Awaiting(() => _service.CriarBeneficiariosAsync(dto))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("CPF já cadastrado");


        _repositoryMock.Verify(
            r => r.AdicionarBeneficiarioAsync(It.IsAny<Beneficiario>()),
            Times.Never
        );
    }



    [Fact]
    public async Task ObterBeneficiarioPorIdAsync_QuandoExiste_DeveRetornarDto()
    {

        var beneficiarioId = Guid.NewGuid();
        var planoId = Guid.NewGuid();


        var beneficiario = new Beneficiario
        {
            Id = beneficiarioId,
            Nome = "Pedro Santos",
            CPF = "98765432100",
            DataNascimento = new DateTime(1995, 8, 10),
            Status = StatusBeneficiario.Ativo,
            PlanoId = planoId,
            Plano = new Plano { Id = planoId, NomePlano = "Plano Gold" },
            DataCadastro = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow
        };


        _repositoryMock
            .Setup(r => r.ObterBeneficiarioPorIdAsync(beneficiarioId))
            .ReturnsAsync(beneficiario);


        var resultado = await _service.ObterBeneficiarioPorIdAsync(beneficiarioId);


        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(beneficiarioId);
        resultado.Nome.Should().Be("Pedro Santos");
        resultado.NomePlano.Should().Be("Plano Gold");
    }

    [Fact]
    public async Task ObterBeneficiarioPorIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {

        var idInexistente = Guid.NewGuid();


        _repositoryMock
            .Setup(r => r.ObterBeneficiarioPorIdAsync(idInexistente))
            .ReturnsAsync((Beneficiario?)null);


        var resultado = await _service.ObterBeneficiarioPorIdAsync(idInexistente);


        resultado.Should().BeNull();
    }



    [Fact]
    public async Task ObterTodosBeneficiariosAsync_SemFiltros_DeveRetornarTodos()
    {

        var planoId = Guid.NewGuid();
        var beneficiarios = new List<Beneficiario>
        {
            new Beneficiario
            {
                Id = Guid.NewGuid(),
                Nome = "Ana",
                CPF = "11111111111",
                Status = StatusBeneficiario.Ativo,
                PlanoId = planoId
            },
            new Beneficiario
            {
                Id = Guid.NewGuid(),
                Nome = "Bruno",
                CPF = "22222222222",
                Status = StatusBeneficiario.Inativo,
                PlanoId = planoId
            }
        };

        _repositoryMock
            .Setup(r => r.ObterBeneficiarioComFiltrosAsync(null, null))
            .ReturnsAsync(beneficiarios);


        var resultado = await _service.ObterTodosBeneficiariosAsync(null, null);


        resultado.Should().HaveCount(2);
        resultado.First().Nome.Should().Be("Ana");
    }



    [Fact]
    public async Task ObterTodosBeneficiariosAsync_ComFiltroStatus_DeveRetornarFiltrados()
    {

        var beneficiariosAtivos = new List<Beneficiario>
        {
            new Beneficiario
            {
                Id = Guid.NewGuid(),
                Nome = "Ana Ativa",
                CPF = "11111111111",
                Status = StatusBeneficiario.Ativo,
                PlanoId = Guid.NewGuid()
            }
        };


        _repositoryMock
            .Setup(r => r.ObterBeneficiarioComFiltrosAsync(StatusBeneficiario.Ativo, null))
            .ReturnsAsync(beneficiariosAtivos);


        var resultado = await _service.ObterTodosBeneficiariosAsync("Ativo", null);


        resultado.Should().HaveCount(1);
        resultado.First().Status.Should().Be("Ativo");
    }



    [Fact]
    public async Task AtualizarBeneficiarioAsync_QuandoExiste_DeveRetornarDtoAtualizado()
    {

        var beneficiarioId = Guid.NewGuid();
        var planoId = Guid.NewGuid();

        var beneficiarioExistente = new Beneficiario
        {
            Id = beneficiarioId,
            Nome = "Nome Antigo",
            CPF = "12345678901",
            DataNascimento = new DateTime(1990, 1, 1),
            Status = StatusBeneficiario.Ativo,
            PlanoId = planoId,
            DataCadastro = DateTime.UtcNow.AddDays(-10),
            DataAtualizacao = DateTime.UtcNow.AddDays(-5)
        };

        var dtoAtualizacao = new UpdateBeneficiarioDto
        {
            Nome = "Nome Novo",
            DataNascimento = new DateTime(1990, 1, 1),
            Status = "Inativo",
            PlanoId = planoId
        };

        _repositoryMock
            .Setup(r => r.ObterBeneficiarioPorIdAsync(beneficiarioId))
            .ReturnsAsync(beneficiarioExistente);

        _repositoryMock
            .Setup(r => r.AtualizarBeneficiarioAsync(It.IsAny<Beneficiario>()))
            .ReturnsAsync((Beneficiario b) => b);


        var resultado = await _service.AtualizarBeneficiarioAsync(beneficiarioId, dtoAtualizacao);


        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Nome Novo");
        resultado.Status.Should().Be("Inativo");
    }


    [Fact]
    public async Task AtualizarBeneficiarioAsync_QuandoNaoExiste_DeveRetornarNull()
    {

        var idInexistente = Guid.NewGuid();
        var dto = new UpdateBeneficiarioDto
        {
            Nome = "Qualquer",
            DataNascimento = DateTime.Now,
            Status = "Ativo",
            PlanoId = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.ObterBeneficiarioPorIdAsync(idInexistente))
            .ReturnsAsync((Beneficiario?)null);


        var resultado = await _service.AtualizarBeneficiarioAsync(idInexistente, dto);


        resultado.Should().BeNull();


        _repositoryMock.Verify(
            r => r.AtualizarBeneficiarioAsync(It.IsAny<Beneficiario>()),
            Times.Never
        );
    }


    [Fact]
    public async Task ExcluirBeneficiarioSuavementeAsync_QuandoExiste_DeveRetornarTrue()
    {

        var beneficiarioId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.ExcluirBeneficiarioSuavementeAsync(beneficiarioId))
            .ReturnsAsync(true);


        var resultado = await _service.ExcluirBeneficiarioSuavementeAsync(beneficiarioId);

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ExcluirBeneficiarioSuavementeAsync_QuandoNaoExiste_DeveRetornarFalse()
    {

        var idInexistente = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.ExcluirBeneficiarioSuavementeAsync(idInexistente))
            .ReturnsAsync(false);


        var resultado = await _service.ExcluirBeneficiarioSuavementeAsync(idInexistente);

        resultado.Should().BeFalse();
    }
}