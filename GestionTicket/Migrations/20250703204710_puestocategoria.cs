using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicket.Migrations
{
    /// <inheritdoc />
    public partial class puestocategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_PuestoLaborales_PuestoLaboralID",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PuestoLaboralID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PuestoLaboralID",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "PuestoCategorias",
                columns: table => new
                {
                    PuestoCategoriaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaID = table.Column<int>(type: "int", nullable: false),
                    PuestoLaboralID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuestoCategorias", x => x.PuestoCategoriaID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PuestoCategorias");

            migrationBuilder.AddColumn<int>(
                name: "PuestoLaboralID",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PuestoLaboralID",
                table: "Tickets",
                column: "PuestoLaboralID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_PuestoLaborales_PuestoLaboralID",
                table: "Tickets",
                column: "PuestoLaboralID",
                principalTable: "PuestoLaborales",
                principalColumn: "PuestoLaboralID");
        }
    }
}
