// IDeudaRepository.cs
// Contrato para la persistencia de deudas (cuentas por cobrar).
// ObtenerPorAlumno permite consultar todas las deudas de un alumno.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IDeudaRepository
{
    Deuda? ObtenerPorId(int id);
    List<Deuda> ObtenerPorAlumno(int alumnoId);
    void Guardar(Deuda deuda);
}
