// ICursoRepository.cs
// Contrato para la persistencia de cursos (libros/niveles).
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface ICursoRepository : IRepository<Curso>
{
    List<Curso> ObtenerTodos();
}
