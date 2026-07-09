using Backend.Contracts;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Tests.Services;

public class TareasServiceTests : TestBase
{
    private readonly TareasService _sut;

    public TareasServiceTests()
    {
        _sut = new TareasService(DbContext);
    }

    [Fact]
    public async Task CrearAsync_ConDatosValidos_CreaTarea()
    {
        // Arrange
        var request = new CrearActualizarTareaRequest
        {
            Titulo = "Tarea de prueba",
            EstaCompletada = false,
            FechaVencimiento = DateTime.UtcNow.AddDays(7),
            Notas = "Notas de prueba",
            Prioridad = PrioridadTarea.Alta,
            EsRepetitiva = false,
            TipoRecurrencia = null,
            ProximaRecurrencia = null,
            PlantillaTareaId = null,
            CategoriaId = null,
            UsuarioId = null,
            TipoTareaId = 1
        };

        // Act
        var resultado = await _sut.CrearAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Titulo.Should().Be("Tarea de prueba");
        resultado.EstaCompletada.Should().BeFalse();
        resultado.Prioridad.Should().Be(PrioridadTarea.Alta);

        var tareaEnBd = await DbContext.Tareas.FindAsync(resultado.Id);
        tareaEnBd.Should().NotBeNull();
    }

    [Fact]
    public async Task CrearAsync_ConUsuarioInexistente_LanzaArgumentException()
    {
        // Arrange
        var request = new CrearActualizarTareaRequest
        {
            Titulo = "Tarea de prueba",
            EstaCompletada = false,
            TipoTareaId = 1,
            UsuarioId = 999 // ID que no existe
        };

        // Act
        var act = async () => await _sut.CrearAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*usuario*no existe*");
    }

    [Fact]
    public async Task CompletarAsync_ConTareaNoRepetitiva_MarcaComoCompletada()
    {
        // Arrange
        var tarea = new Tarea
        {
            Titulo = "Tarea simple",
            EstaCompletada = false,
            FechaCreacion = DateTime.UtcNow,
            EsRepetitiva = false,
            TipoRecurrencia = null,
            TipoTareaId = 1
        };
        DbContext.Tareas.Add(tarea);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        // Act
        var resultado = await _sut.CompletarAsync(tarea.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.EstaCompletada.Should().BeTrue();

        var tareaEnBd = await DbContext.Tareas.FindAsync(tarea.Id);
        tareaEnBd!.EstaCompletada.Should().BeTrue();
    }

    [Fact]
    public async Task CompletarAsync_ConTareaRepetitiva_CreaSiguienteOcurrencia()
    {
        // Arrange
        var tarea = new Tarea
        {
            Titulo = "Tarea repetitiva",
            EstaCompletada = false,
            FechaCreacion = DateTime.UtcNow,
            EsRepetitiva = true,
            TipoRecurrencia = TipoRecurrencia.Diaria,
            ProximaRecurrencia = DateTime.UtcNow.AddDays(1),
            TipoTareaId = 1
        };
        DbContext.Tareas.Add(tarea);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var tareasAntesDeCompletar = await DbContext.Tareas.CountAsync();

        // Act
        var resultado = await _sut.CompletarAsync(tarea.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.EstaCompletada.Should().BeTrue();

        var tareasDespuesDeCompletar = await DbContext.Tareas.CountAsync();
        tareasDespuesDeCompletar.Should().Be(tareasAntesDeCompletar + 1);

        var siguienteTarea = await DbContext.Tareas
            .Where(t => t.Id != tarea.Id)
            .FirstOrDefaultAsync();

        siguienteTarea.Should().NotBeNull();
        siguienteTarea!.Titulo.Should().Be("Tarea repetitiva");
        siguienteTarea.EstaCompletada.Should().BeFalse();
        siguienteTarea.EsRepetitiva.Should().BeTrue();
        siguienteTarea.TipoRecurrencia.Should().Be(TipoRecurrencia.Diaria);
    }

    [Fact]
    public async Task CompletarAsync_TareaYaCompletada_NoCreaDuplicados()
    {
        // Arrange
        var tarea = new Tarea
        {
            Titulo = "Tarea ya completada",
            EstaCompletada = true,
            FechaCreacion = DateTime.UtcNow,
            EsRepetitiva = true,
            TipoRecurrencia = TipoRecurrencia.Semanal,
            TipoTareaId = 1
        };
        DbContext.Tareas.Add(tarea);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        var tareasAntes = await DbContext.Tareas.CountAsync();

        // Act
        var resultado = await _sut.CompletarAsync(tarea.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.EstaCompletada.Should().BeTrue();

        var tareasDespues = await DbContext.Tareas.CountAsync();
        tareasDespues.Should().Be(tareasAntes);
    }

    [Fact]
    public async Task CompletarAsync_ConIdInexistente_DevuelveNull()
    {
        // Act
        var resultado = await _sut.CompletarAsync(999);

        // Assert
        resultado.Should().BeNull();
    }
}
