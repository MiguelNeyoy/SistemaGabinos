// ICursoRepository.cs
// Contrato para la persistencia de cursos (libros/niveles).
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface ICursoRepository
{
    Curso? ObtenerPorId(int id);
    List<Curso> ObtenerTodos();
    void Guardar(Curso curso);
}
