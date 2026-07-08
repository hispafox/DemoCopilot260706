using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Tarea> Tareas => Set<Tarea>();

    public DbSet<PlantillaTarea> PlantillasTarea => Set<PlantillaTarea>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Departamento> Departamentos => Set<Departamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Prioridad).HasConversion<int>();

            entity.HasOne(t => t.Categoria)
                .WithMany(c => c.Tareas)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.PlantillaTarea)
                .WithMany()
                .HasForeignKey(t => t.PlantillaTareaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PlantillaTarea>(entity =>
        {
            entity.Property(p => p.Titulo).IsRequired().HasMaxLength(200);

            entity.HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Color).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(u => u.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Email).HasMaxLength(200);

            entity.HasOne(u => u.Departamento)
                .WithMany(d => d.Usuarios)
                .HasForeignKey(u => u.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
        });
    }
}
