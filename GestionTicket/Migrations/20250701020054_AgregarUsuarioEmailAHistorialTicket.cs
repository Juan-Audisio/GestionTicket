using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTicket.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUsuarioEmailAHistorialTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioClienteID",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioEmail",
                table: "HistorialTicket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UsuarioClienteID",
                table: "Tickets",
                column: "UsuarioClienteID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_UsuarioClienteID",
                table: "Tickets",
                column: "UsuarioClienteID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_UsuarioClienteID",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UsuarioClienteID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "UsuarioEmail",
                table: "HistorialTicket");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioClienteID",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
