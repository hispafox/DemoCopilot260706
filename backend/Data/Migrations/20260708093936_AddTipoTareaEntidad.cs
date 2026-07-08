using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoTareaEntidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoTareaId",
                table: "Tareas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.CreateTable(
                name: "TiposTarea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    EstaActivo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTarea", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TiposTarea",
                columns: new[] { "Id", "Descripcion", "EstaActivo", "Nombre" },
                values: new object[,]
                {
                    { 1, null, true, "Proyecto" },
                    { 2, null, true, "Objetivo" },
                    { 3, null, true, "Tarea" },
                    { 4, null, true, "Hito" }
                });

            migrationBuilder.Sql("UPDATE Tareas SET TipoTareaId = 3 WHERE TipoTareaId = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_TipoTareaId",
                table: "Tareas",
                column: "TipoTareaId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposTarea_Nombre",
                table: "TiposTarea",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_TiposTarea_TipoTareaId",
                table: "Tareas",
                column: "TipoTareaId",
                principalTable: "TiposTarea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_TiposTarea_TipoTareaId",
                table: "Tareas");

            migrationBuilder.DropTable(
                name: "TiposTarea");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_TipoTareaId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "TipoTareaId",
                table: "Tareas");
        }
    }
}
