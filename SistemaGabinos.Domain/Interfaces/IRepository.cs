// IRepository.cs
// Interfaz genérica con operaciones base de persistencia.
namespace SistemaGabinos.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    T? ObtenerPorId(int id);
    void Guardar(T entity);
}
