using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGabinos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCondicionesFinancieras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostoMensualidadPactada",
                table: "Alumnos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 1400m);

            migrationBuilder.AddColumn<decimal>(
                name: "DescuentoBecaPactada",
                table: "Alumnos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("UPDATE Alumnos SET DescuentoBecaPactada = 400.0 WHERE TieneBeca = 1;");
            migrationBuilder.Sql("UPDATE Alumnos SET CostoMensualidadPactada = 1400.0 WHERE CostoMensualidadPactada = 0;");

            migrationBuilder.DropColumn(
                name: "TieneBeca",
                table: "Alumnos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostoMensualidadPactada",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "DescuentoBecaPactada",
                table: "Alumnos");

            migrationBuilder.AddColumn<bool>(
                name: "TieneBeca",
                table: "Alumnos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
