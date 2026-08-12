// Curso.cs
// Representa un nivel o libro en la escuela (Book 1, Book 2, etc.).
// No hay grados escolares tradicionales — los alumnos avanzan por libros/niveles.
// PrecioLibro es el costo del libro que se cobra al pasar este nivel.
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Domain.Entities;

public class Curso
{
    public int Id { get; private set; }
    public NivelCurso Nivel { get; private set; }
    public decimal PrecioLibro { get; private set; }

    public string Nombre => Nivel switch
    {
        NivelCurso.Book1 => "Book 1",
        NivelCurso.Book2 => "Book 2",
        NivelCurso.Book3 => "Book 3",
        NivelCurso.Book4 => "Book 4",
        NivelCurso.Book5 => "Book 5",
        NivelCurso.Book6 => "Book 6",
        _ => Nivel.ToString()
    };

    private Curso() { }

    public Curso(int id, NivelCurso nivel, decimal precioLibro)
        : this(nivel, precioLibro)
    {
        Id = id;
    }

    public Curso(NivelCurso nivel, decimal precioLibro)
    {
        if (!Enum.IsDefined(typeof(NivelCurso), nivel))
            throw new ArgumentException("El nivel del curso no es válido.", nameof(nivel));

        if (precioLibro <= 0)
            throw new ArgumentException("El precio del libro debe ser mayor que cero.", nameof(precioLibro));

        Nivel = nivel;
        PrecioLibro = precioLibro;
    }

    public Curso(int id, string nombre, decimal precioLibro)
    {
        Id = id;
        Nivel = Enum.TryParse<NivelCurso>(nombre.Replace(" ", ""), true, out var parsed) ? parsed : NivelCurso.Book1;
        PrecioLibro = precioLibro;
    }

    public Curso(string nombre, decimal precioLibro)
    {
        Nivel = Enum.TryParse<NivelCurso>(nombre.Replace(" ", ""), true, out var parsed) ? parsed : NivelCurso.Book1;
        PrecioLibro = precioLibro;
    }
}
