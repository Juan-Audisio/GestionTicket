using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicket.Migrations
{
    /// <inheritdoc />
    public partial class desarrolladores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Desarrolladores_PuestoLaboralID",
                table: "Desarrolladores",
                column: "PuestoLaboralID");

            migrationBuilder.AddForeignKey(
                name: "FK_Desarrolladores_PuestoLaborales_PuestoLaboralID",
                table: "Desarrolladores",
                column: "PuestoLaboralID",
                principalTable: "PuestoLaborales",
                principalColumn: "PuestoLaboralID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desarrolladores_PuestoLaborales_PuestoLaboralID",
                table: "Desarrolladores");

            migrationBuilder.DropIndex(
                name: "IX_Desarrolladores_PuestoLaboralID",
                table: "Desarrolladores");
        }
    }
}
