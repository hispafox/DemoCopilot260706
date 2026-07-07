using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EstaActiva = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasTarea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    EsRepetitiva = table.Column<bool>(type: "INTEGER", nullable: false),
                    TipoRecurrencia = table.Column<int>(type: "INTEGER", nullable: true),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: true),
                    EstaActiva = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasTarea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantillasTarea_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tareas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EstaCompletada = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    Prioridad = table.Column<int>(type: "INTEGER", nullable: false),
                    EsRepetitiva = table.Column<bool>(type: "INTEGER", nullable: false),
                    TipoRecurrencia = table.Column<int>(type: "INTEGER", nullable: true),
                    ProximaRecurrencia = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlantillaTareaId = table.Column<int>(type: "INTEGER", nullable: true),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tareas_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Tareas_PlantillasTarea_PlantillaTareaId",
                        column: x => x.PlantillaTareaId,
                        principalTable: "PlantillasTarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasTarea_CategoriaId",
                table: "PlantillasTarea",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_CategoriaId",
                table: "Tareas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_PlantillaTareaId",
                table: "Tareas",
                column: "PlantillaTareaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tareas");

            migrationBuilder.DropTable(
                name: "PlantillasTarea");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
