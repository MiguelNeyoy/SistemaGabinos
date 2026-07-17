using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaGabinos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCursos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cursos",
                columns: new[] { "Id", "Nombre", "PrecioLibro" },
                values: new object[,]
                {
                    { 1, "Book 1", 350m },
                    { 2, "Book 2", 350m },
                    { 3, "Book 3", 350m },
                    { 4, "Book 4", 350m },
                    { 5, "Book 5", 350m },
                    { 6, "Book 6", 350m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cursos",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
