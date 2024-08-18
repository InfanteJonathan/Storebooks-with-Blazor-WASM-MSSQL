using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBooksBlazorWASM.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldUsuaruoVentas10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUsuario",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Ventas");
        }
    }
}
