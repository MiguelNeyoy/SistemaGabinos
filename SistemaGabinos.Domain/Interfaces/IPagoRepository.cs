// IPagoRepository.cs
// Contrato para la persistencia de pagos.
// ObtenerPorAlumno consulta todos los pagos de un alumno.
// ObtenerPorDeuda consulta los pagos parciales asociados a una deuda.
using SistemaGabinos.Domain.Entities;

namespace SistemaGabinos.Domain.Interfaces;

public interface IPagoRepository
{
    Pago? ObtenerPorId(int id);
    List<Pago> ObtenerPorAlumno(int alumnoId);
    List<Pago> ObtenerPorDeuda(int deudaId);
    void Guardar(Pago pago);
}
