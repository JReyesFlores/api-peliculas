using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class AddFechaModificacionPelicula : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "Pelicula",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "Pelicula");
        }
    }
}
