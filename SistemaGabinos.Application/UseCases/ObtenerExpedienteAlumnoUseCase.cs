using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ObtenerExpedienteAlumnoUseCase : IObtenerExpedienteAlumnoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly IPagoRepository _pagoRepo;

    public ObtenerExpedienteAlumnoUseCase(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo,
        IPagoRepository pagoRepo)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
        _pagoRepo = pagoRepo;
    }

    public ExpedienteAlumnoDto? Ejecutar(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId);
        if (alumno is null)
            return null;

        var deudas = _deudaRepo.ObtenerPorAlumno(alumnoId);
        var pagos = _pagoRepo.ObtenerPorAlumno(alumnoId);

        var pagoItems = new List<PagoItem>();

        foreach (var pago in pagos)
        {
            pagoItems.Add(new PagoItem
            {
                Folio = $"#INV-{pago.Id:D4}",
                Concepto = pago.Concepto.ToString(),
                Monto = pago.Monto,
                Estado = pago.EstaCancelado ? "Cancelado" : "Pagado",
                Fecha = pago.Fecha
            });
        }

        foreach (var deuda in deudas.Where(d => !d.EstaPagada))
        {
            var saldoPendiente = deuda.MontoTotal - deuda.MontoPagado;
            pagoItems.Add(new PagoItem
            {
                Folio = $"#DEU-{deuda.Id:D4}",
                Concepto = $"{deuda.Concepto} (Saldo Pendiente)",
                Monto = saldoPendiente,
                Estado = "Pendiente",
                Fecha = deuda.FechaCreacion
            });
        }

        var totalPendiente = deudas
            .Where(d => !d.EstaPagada)
            .Sum(d => d.MontoTotal - d.MontoPagado);

        return new ExpedienteAlumnoDto
        {
            Id = alumno.Id,
            NombreCompleto = alumno.NombreCompleto,
            Curp = alumno.CURP,
            FechaNacimiento = alumno.FechaNacimiento,
            Telefono = alumno.Telefono,
            NombreTutor = alumno.NombreTutor,
            ParentescoTutor = alumno.ParentescoTutor,
            TelefonoTutor = alumno.TelefonoTutor,
            Estado = alumno.Estado.ToString(),
            Pagos = pagoItems.OrderByDescending(p => p.Fecha).ToList(),
            TotalPendiente = totalPendiente
        };
    }
}
