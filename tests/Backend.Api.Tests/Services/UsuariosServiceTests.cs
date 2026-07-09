using Backend.Contracts;
using Backend.Models;
using Backend.Services;
using FluentAssertions;

namespace Backend.Api.Tests.Services;

public class UsuariosServiceTests : TestBase
{
    private readonly UsuariosService _sut;

    public UsuariosServiceTests()
    {
        _sut = new UsuariosService(DbContext);
    }

    [Fact]
    public async Task CrearAsync_ConDepartamentoExistente_CreaUsuario()
    {
        // Arrange
        var request = new CrearActualizarUsuarioRequest
        {
            Nombre = "Usuario Test",
            Email = "test@ejemplo.com",
            DepartamentoId = 1,
            SedeId = 1,
            PoblacionId = 1
        };

        // Act
        var resultado = await _sut.CrearAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Nombre.Should().Be("Usuario Test");
        resultado.Email.Should().Be("test@ejemplo.com");
        resultado.DepartamentoId.Should().Be(1);

        var usuarioEnBd = await DbContext.Usuarios.FindAsync(resultado.Id);
        usuarioEnBd.Should().NotBeNull();
    }

    [Fact]
    public async Task CrearAsync_ConDepartamentoInexistente_LanzaArgumentException()
    {
        // Arrange
        var request = new CrearActualizarUsuarioRequest
        {
            Nombre = "Usuario Test",
            Email = "test@ejemplo.com",
            DepartamentoId = 999, // ID que no existe
            SedeId = 1,
            PoblacionId = 1
        };

        // Act
        var act = async () => await _sut.CrearAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*departamento*no existe*");
    }

    [Fact]
    public async Task CrearAsync_ConSedeInexistente_LanzaArgumentException()
    {
        // Arrange
        var request = new CrearActualizarUsuarioRequest
        {
            Nombre = "Usuario Test",
            Email = "test@ejemplo.com",
            DepartamentoId = 1,
            SedeId = 999, // ID que no existe
            PoblacionId = 1
        };

        // Act
        var act = async () => await _sut.CrearAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*sede*no existe*");
    }

    [Fact]
    public async Task ActualizarAsync_ConDepartamentoExistente_ActualizaUsuario()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nombre = "Usuario Original",
            Email = "original@ejemplo.com",
            DepartamentoId = 1,
            SedeId = 1,
            PoblacionId = 1
        };
        DbContext.Usuarios.Add(usuario);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var request = new CrearActualizarUsuarioRequest
        {
            Nombre = "Usuario Actualizado",
            Email = "actualizado@ejemplo.com",
            DepartamentoId = 1,
            SedeId = 1,
            PoblacionId = 1
        };

        // Act
        var resultado = await _sut.ActualizarAsync(usuario.Id, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Usuario Actualizado");
        resultado.Email.Should().Be("actualizado@ejemplo.com");
    }

    [Fact]
    public async Task ActualizarAsync_ConDepartamentoInexistente_LanzaArgumentException()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nombre = "Usuario Original",
            Email = "original@ejemplo.com",
            DepartamentoId = 1,
            SedeId = 1,
            PoblacionId = 1
        };
        DbContext.Usuarios.Add(usuario);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var request = new CrearActualizarUsuarioRequest
        {
            Nombre = "Usuario Actualizado",
            Email = "actualizado@ejemplo.com",
            DepartamentoId = 999, // ID que no existe
            SedeId = 1,
            PoblacionId = 1
        };

        // Act
        var act = async () => await _sut.ActualizarAsync(usuario.Id, request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*departamento*no existe*");
    }

    [Fact]
    public async Task ActualizarAsync_ConIdInexistente_DevuelveNull()
    {
        // Arrange
        var request = new CrearActualizarUsuarioRequest
        {
            Nombre = "Usuario Test",
            Email = "test@ejemplo.com",
            DepartamentoId = 1,
            SedeId = 1,
            PoblacionId = 1
        };

        // Act
        var resultado = await _sut.ActualizarAsync(999, request);

        // Assert
        resultado.Should().BeNull();
    }
}
