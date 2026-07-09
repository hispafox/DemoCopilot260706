using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPoblacionCodigoIsoPais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoIsoPais",
                table: "Poblaciones",
                type: "TEXT",
                maxLength: 2,
                nullable: false,
                defaultValue: "ES");

            migrationBuilder.CreateIndex(
                name: "IX_Poblaciones_CodigoIsoPais",
                table: "Poblaciones",
                column: "CodigoIsoPais");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Poblaciones_CodigoIsoPais",
                table: "Poblaciones");

            migrationBuilder.DropColumn(
                name: "CodigoIsoPais",
                table: "Poblaciones");
        }
    }
}
