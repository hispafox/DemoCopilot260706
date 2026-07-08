using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSedes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO Sedes (Id, Nombre) VALUES (1, 'Central');");

            migrationBuilder.AddColumn<int>(
                name: "SedeId",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("UPDATE Usuarios SET SedeId = 1 WHERE SedeId IS NULL OR SedeId = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_SedeId",
                table: "Usuarios",
                column: "SedeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Sedes_SedeId",
                table: "Usuarios",
                column: "SedeId",
                principalTable: "Sedes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Sedes_SedeId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Sedes");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_SedeId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SedeId",
                table: "Usuarios");
        }
    }
}
