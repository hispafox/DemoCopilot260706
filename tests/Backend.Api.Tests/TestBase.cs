using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Api.Tests;

public abstract class TestBase : IDisposable
{
    protected ApplicationDbContext DbContext { get; }

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new ApplicationDbContext(options);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var departamento = new Departamento { Id = 1, Nombre = "Departamento Test" };
        var sede = new Sede { Id = 1, Nombre = "Sede Test" };
        var poblacion = new Poblacion { Id = 1, Nombre = "Poblacion Test" };
        var tipoTarea = new TipoTarea { Id = 1, Nombre = "Tarea", EstaActivo = true };

        DbContext.Departamentos.Add(departamento);
        DbContext.Sedes.Add(sede);
        DbContext.Poblaciones.Add(poblacion);
        DbContext.TiposTarea.Add(tipoTarea);
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
