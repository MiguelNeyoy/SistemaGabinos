using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGabinos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", nullable: false),
                    CURP = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: false),
                    NombreTutor = table.Column<string>(type: "TEXT", nullable: true),
                    ParentescoTutor = table.Column<string>(type: "TEXT", nullable: true),
                    TelefonoTutor = table.Column<string>(type: "TEXT", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    PrecioLibro = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deudas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlumnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deudas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlumnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CursoId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlumnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeudaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", nullable: false),
                    EstaCancelado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recibos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Folio = table.Column<string>(type: "TEXT", nullable: false),
                    Detalle = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recibos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_CURP",
                table: "Alumnos",
                column: "CURP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_AlumnoId_CursoId",
                table: "Inscripciones",
                columns: new[] { "AlumnoId", "CursoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recibos_Folio",
                table: "Recibos",
                column: "Folio",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Deudas");

            migrationBuilder.DropTable(
                name: "Inscripciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Recibos");
        }
    }
}
