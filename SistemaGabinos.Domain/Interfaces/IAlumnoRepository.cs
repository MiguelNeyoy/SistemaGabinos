// IAlumnoRepository.cs
// Contrato para la persistencia de alumnos.
// ObtenerPorCURP es usado en el flujo de inscripción para validar duplicados.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IAlumnoRepository
{
    Alumno? ObtenerPorId(int id);
    Alumno? ObtenerPorCURP(string curp);
    List<Alumno> ObtenerTodos();
    void Guardar(Alumno alumno);
    void Eliminar(int id);
}
