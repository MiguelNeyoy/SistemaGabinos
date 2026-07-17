// Curso.cs
// Representa un nivel o libro en la escuela (Book 1, Book 2, etc.).
// No hay grados escolares tradicionales — los alumnos avanzan por libros/niveles.
// PrecioLibro es el costo del libro que se cobra al pasar este nivel.
namespace SistemaGabinos.Domain.Entities;

public class Curso
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public decimal PrecioLibro { get; private set; }

    private Curso() { }

    public Curso(int id, string nombre, decimal precioLibro)
        : this(nombre, precioLibro)
    {
        Id = id;
    }

    public Curso(string nombre, decimal precioLibro)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del curso no puede estar vacío.", nameof(nombre));

        if (precioLibro <= 0)
            throw new ArgumentException("El precio del libro debe ser mayor que cero.", nameof(precioLibro));

        Nombre = nombre;
        PrecioLibro = precioLibro;
    }
}
