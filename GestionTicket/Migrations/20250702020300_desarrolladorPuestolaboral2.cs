using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicket.Migrations
{
    /// <inheritdoc />
    public partial class desarrolladorPuestolaboral2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PuestoLaboralID",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Desarrolladores",
                columns: table => new
                {
                    DesarrolladorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DNI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PuestoLaboralID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioClienteID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desarrolladores", x => x.DesarrolladorID);
                });

            migrationBuilder.CreateTable(
                name: "PuestoLaborales",
                columns: table => new
                {
                    PuestoLaboralID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioClienteID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuestoLaborales", x => x.PuestoLaboralID);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_PuestoLaborales_PuestoLaboralID",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Desarrolladores");

            migrationBuilder.DropTable(
                name: "PuestoLaborales");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_PuestoLaboralID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PuestoLaboralID",
                table: "Tickets");
        }
    }
}
