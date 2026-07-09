using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPoblacionProvincia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "Poblaciones",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Poblaciones_Provincia",
                table: "Poblaciones",
                column: "Provincia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Poblaciones_Provincia",
                table: "Poblaciones");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "Poblaciones");
        }
    }
}
