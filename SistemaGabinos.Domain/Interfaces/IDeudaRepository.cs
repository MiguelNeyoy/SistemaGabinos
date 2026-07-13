// IDeudaRepository.cs
// Contrato para la persistencia de deudas (cuentas por cobrar).
// ObtenerPorAlumno permite consultar todas las deudas de un alumno.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IDeudaRepository : IRepository<Deuda>
{
    List<Deuda> ObtenerPorAlumno(int alumnoId);
}
