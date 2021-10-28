using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class AgregandoColumnaDuracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duracion",
                table: "Pelicula",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Pelicula");
        }
    }
}
