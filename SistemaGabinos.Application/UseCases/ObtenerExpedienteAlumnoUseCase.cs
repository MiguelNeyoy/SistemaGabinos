using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ObtenerExpedienteAlumnoUseCase : IObtenerExpedienteAlumnoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly IPagoRepository _pagoRepo;
    private readonly IInscripcionRepository _inscripcionRepo;
    private readonly ICursoRepository _cursoRepo;

    public ObtenerExpedienteAlumnoUseCase(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo,
        IPagoRepository pagoRepo,
        IInscripcionRepository inscripcionRepo,
        ICursoRepository cursoRepo)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
        _pagoRepo = pagoRepo;
        _inscripcionRepo = inscripcionRepo;
        _cursoRepo = cursoRepo;
    }

    public ExpedienteAlumnoDto? Ejecutar(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId);
        if (alumno is null)
            return null;

        var deudas = _deudaRepo.ObtenerPorAlumno(alumnoId);
        var pagos = _pagoRepo.ObtenerPorAlumno(alumnoId);
        var inscripciones = _inscripcionRepo.ObtenerPorAlumno(alumnoId);
        var inscripcionVigente = inscripciones.FirstOrDefault(i => i.Estado == Domain.Enums.EstadoInscripcion.Vigente)
                                 ?? inscripciones.LastOrDefault();

        string horarioStr = inscripcionVigente?.Horario.ToString() ?? "Mañana";
        string cursoStr = "Sin Curso";

        if (inscripcionVigente is not null)
        {
            var curso = _cursoRepo.ObtenerPorId(inscripcionVigente.CursoId);
            if (curso is not null)
                cursoStr = curso.Nombre;
        }

        var pagoItems = new List<PagoItem>();

        foreach (var pago in pagos)
        {
            pagoItems.Add(new PagoItem
            {
                Folio = SistemaGabinos.Domain.Enums.TipoFolio.Pago.Formatear(pago.Id),
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
                Folio = SistemaGabinos.Domain.Enums.TipoFolio.Deuda.Formatear(deuda.Id),
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
            TieneBeca = alumno.TieneBeca,
            Horario = horarioStr,
            CursoActual = cursoStr,
            Pagos = pagoItems.OrderByDescending(p => p.Fecha).ToList(),
            TotalPendiente = totalPendiente
        };
    }
}
