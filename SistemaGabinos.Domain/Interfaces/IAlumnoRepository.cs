// IAlumnoRepository.cs
// Contrato para la persistencia de alumnos.
// ObtenerPorCURP es usado en el flujo de inscripción para validar duplicados.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IAlumnoRepository : IRepository<Alumno>
{
    Alumno? ObtenerPorCURP(string curp);
    List<Alumno> BuscarPorNombreOCurp(string criterio, int maxResultados = 10);
    List<Alumno> ObtenerTodos();
    void Eliminar(int id);
}
