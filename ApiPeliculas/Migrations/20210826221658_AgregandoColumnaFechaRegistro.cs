using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class AgregandoColumnaFechaRegistro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Usuario",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Usuario");
        }
    }
}
