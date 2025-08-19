using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicket.Migrations
{
    /// <inheritdoc />
    public partial class puestocategoria1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PuestoCategorias_CategoriaID",
                table: "PuestoCategorias",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "IX_PuestoCategorias_PuestoLaboralID",
                table: "PuestoCategorias",
                column: "PuestoLaboralID");

            migrationBuilder.AddForeignKey(
                name: "FK_PuestoCategorias_Categorias_CategoriaID",
                table: "PuestoCategorias",
                column: "CategoriaID",
                principalTable: "Categorias",
                principalColumn: "CategoriaID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PuestoCategorias_PuestoLaborales_PuestoLaboralID",
                table: "PuestoCategorias",
                column: "PuestoLaboralID",
                principalTable: "PuestoLaborales",
                principalColumn: "PuestoLaboralID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PuestoCategorias_Categorias_CategoriaID",
                table: "PuestoCategorias");

            migrationBuilder.DropForeignKey(
                name: "FK_PuestoCategorias_PuestoLaborales_PuestoLaboralID",
                table: "PuestoCategorias");

            migrationBuilder.DropIndex(
                name: "IX_PuestoCategorias_CategoriaID",
                table: "PuestoCategorias");

            migrationBuilder.DropIndex(
                name: "IX_PuestoCategorias_PuestoLaboralID",
                table: "PuestoCategorias");
        }
    }
}
