using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPoblacionesYRelacionUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Poblaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poblaciones", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Poblaciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Sin asignar" });

            migrationBuilder.AddColumn<int>(
                name: "PoblacionId",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PoblacionId",
                table: "Usuarios",
                column: "PoblacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Poblaciones_PoblacionId",
                table: "Usuarios",
                column: "PoblacionId",
                principalTable: "Poblaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Poblaciones_PoblacionId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Poblaciones");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PoblacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PoblacionId",
                table: "Usuarios");
        }
    }
}
