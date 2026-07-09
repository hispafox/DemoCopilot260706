using Backend.Contracts;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Tests.Services;

public class DepartamentosServiceTests : TestBase
{
    private readonly DepartamentosService _sut;

    public DepartamentosServiceTests()
    {
        _sut = new DepartamentosService(DbContext);
    }

    [Fact]
    public async Task CrearAsync_ConDatosValidos_CreaDepartamento()
    {
        // Arrange
        var request = new CrearActualizarDepartamentoRequest
        {
            Nombre = "Nuevo Departamento"
        };

        // Act
        var resultado = await _sut.CrearAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Nombre.Should().Be("Nuevo Departamento");

        var departamentoEnBd = await DbContext.Departamentos.FindAsync(resultado.Id);
        departamentoEnBd.Should().NotBeNull();
    }

    [Fact]
    public async Task EliminarAsync_SinUsuariosAsociados_EliminaDepartamento()
    {
        // Arrange
        var departamento = new Departamento { Nombre = "Departamento Sin Usuarios" };
        DbContext.Departamentos.Add(departamento);
        await DbContext.SaveChangesAsync();

        // Act
        var resultado = await _sut.EliminarAsync(departamento.Id);

        // Assert
        resultado.Should().BeTrue();

        var departamentoEnBd = await DbContext.Departamentos.FindAsync(departamento.Id);
        departamentoEnBd.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_ConUsuariosAsociados_NoPuedeEliminar()
    {
        // Arrange
        var departamento = new Departamento { Nombre = "Departamento Con Usuarios" };
        DbContext.Departamentos.Add(departamento);
        await DbContext.SaveChangesAsync();

        var usuario = new Usuario
        {
            Nombre = "Usuario Test",
            Email = "test@ejemplo.com",
            DepartamentoId = departamento.Id,
            SedeId = 1,
            PoblacionId = 1
        };
        DbContext.Usuarios.Add(usuario);
        await DbContext.SaveChangesAsync();

        // Act
        var resultado = await _sut.EliminarAsync(departamento.Id);

        // Assert
        resultado.Should().BeFalse();

        var departamentoEnBd = await DbContext.Departamentos.FindAsync(departamento.Id);
        departamentoEnBd.Should().NotBeNull();
    }

    [Fact]
    public async Task EliminarAsync_ConIdInexistente_DevuelveFalse()
    {
        // Act
        var resultado = await _sut.EliminarAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}
