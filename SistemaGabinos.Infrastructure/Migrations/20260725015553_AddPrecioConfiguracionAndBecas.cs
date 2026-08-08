using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGabinos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecioConfiguracionAndBecas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Cursos",
                newName: "Nivel");

            migrationBuilder.AddColumn<string>(
                name: "Horario",
                table: "Inscripciones",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TieneBeca",
                table: "Alumnos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PreciosConfiguraciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CostoInscripcion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoMensualidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoLibro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoExamenUbicacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoDescuentoBeca = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosConfiguraciones", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nivel",
                value: "Book1");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nivel",
                value: "Book2");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nivel",
                value: "Book3");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nivel",
                value: "Book4");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nivel",
                value: "Book5");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nivel",
                value: "Book6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreciosConfiguraciones");

            migrationBuilder.DropColumn(
                name: "Horario",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "TieneBeca",
                table: "Alumnos");

            migrationBuilder.RenameColumn(
                name: "Nivel",
                table: "Cursos",
                newName: "Nombre");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Book 1");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Book 2");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Book 3");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "Book 4");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Book 5");

            migrationBuilder.UpdateData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nombre",
                value: "Book 6");
        }
    }
}
