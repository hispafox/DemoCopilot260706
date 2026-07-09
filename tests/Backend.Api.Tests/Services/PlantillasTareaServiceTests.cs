using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Tests.Services;

public class PlantillasTareaServiceTests : TestBase
{
    private readonly PlantillasTareaService _plantillasService;
    private readonly TareasService _tareasService;

    public PlantillasTareaServiceTests()
    {
        _plantillasService = new PlantillasTareaService(DbContext);
        _tareasService = new TareasService(DbContext);
    }

    [Fact]
    public async Task CrearDesdePlantilla_GeneraTareaIndependiente()
    {
        // Arrange
        var plantilla = new PlantillaTarea
        {
            Titulo = "Plantilla de prueba",
            Notas = "Notas de plantilla",
            EsRepetitiva = true,
            TipoRecurrencia = TipoRecurrencia.Semanal,
            CategoriaId = null,
            EstaActiva = true
        };
        DbContext.PlantillasTarea.Add(plantilla);
        await DbContext.SaveChangesAsync();

        // Act
        var tarea = await _tareasService.CrearDesdePlantillaAsync(plantilla.Id);

        // Assert
        tarea.Should().NotBeNull();
        tarea!.Titulo.Should().Be("Plantilla de prueba");
        tarea.Notas.Should().Be("Notas de plantilla");
        tarea.EsRepetitiva.Should().BeTrue();
        tarea.TipoRecurrencia.Should().Be(TipoRecurrencia.Semanal);
        tarea.PlantillaTareaId.Should().Be(plantilla.Id);
        tarea.EstaCompletada.Should().BeFalse();
        tarea.Id.Should().BeGreaterThan(0);

        // Verificar que existe en la base de datos como una entidad Tarea
        var tareaEnBd = await DbContext.Tareas.FindAsync(tarea.Id);
        tareaEnBd.Should().NotBeNull();
    }

    [Fact]
    public async Task CrearDesdePlantilla_ModificarTarea_NoAfectaPlantilla()
    {
        // Arrange
        var plantilla = new PlantillaTarea
        {
            Titulo = "Plantilla original",
            Notas = "Notas originales",
            EsRepetitiva = false,
            EstaActiva = true
        };
        DbContext.PlantillasTarea.Add(plantilla);
        await DbContext.SaveChangesAsync();

        var tarea = await _tareasService.CrearDesdePlantillaAsync(plantilla.Id);
        tarea.Should().NotBeNull();

        // Act - Modificar la tarea
        var tareaEnBd = await DbContext.Tareas.FindAsync(tarea!.Id);
        tareaEnBd.Should().NotBeNull();
        tareaEnBd!.Titulo = "Titulo modificado";
        tareaEnBd.Notas = "Notas modificadas";
        await DbContext.SaveChangesAsync();

        // Assert - La plantilla debe seguir igual
        DbContext.ChangeTracker.Clear();
        var plantillaActualizada = await DbContext.PlantillasTarea.FindAsync(plantilla.Id);
        plantillaActualizada.Should().NotBeNull();
        plantillaActualizada!.Titulo.Should().Be("Plantilla original");
        plantillaActualizada.Notas.Should().Be("Notas originales");
    }

    [Fact]
    public async Task CrearDesdePlantilla_ConPlantillaInexistente_DevuelveNull()
    {
        // Act
        var tarea = await _tareasService.CrearDesdePlantillaAsync(999);

        // Assert
        tarea.Should().BeNull();
    }
}
