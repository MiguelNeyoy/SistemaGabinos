// RegistrarPagoUseCase.cs
// Caso de uso para registrar el cobro transaccional en la base de datos.
using FluentValidation;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Application.Validators;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class RegistrarPagoUseCase : IRegistrarPagoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly IPagoRepository _pagoRepo;
    private readonly IReciboRepository _reciboRepo;
    private readonly IInscripcionRepository _inscripcionRepo;
    private readonly ICursoRepository _cursoRepo;
    private readonly RegistrarPagoValidator _validator;

    public RegistrarPagoUseCase(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo,
        IPagoRepository pagoRepo,
        IReciboRepository reciboRepo,
        IInscripcionRepository inscripcionRepo,
        ICursoRepository cursoRepo,
        RegistrarPagoValidator validator)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
        _pagoRepo = pagoRepo;
        _reciboRepo = reciboRepo;
        _inscripcionRepo = inscripcionRepo;
        _cursoRepo = cursoRepo;
        _validator = validator;
    }

    public RegistrarPagoResponse Ejecutar(RegistrarPagoRequest request)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var alumno = _alumnoRepo.ObtenerPorId(request.AlumnoId);
        if (alumno is null)
            throw new DomainException($"No se encontró al alumno con ID {request.AlumnoId}.");

        var deudasAlumno = _deudaRepo.ObtenerPorAlumno(request.AlumnoId);
        var deudasSeleccionadas = deudasAlumno
            .Where(d => request.DeudasSeleccionadasIds.Contains(d.Id) && !d.EstaPagada)
            .ToList();

        if (deudasSeleccionadas.Count == 0)
            throw new DomainException("No hay deudas pendientes seleccionadas para registrar el pago.");

        decimal totalDeudasSeleccionadas = deudasSeleccionadas.Sum(d => d.MontoTotal - d.MontoPagado);
        decimal montoAAplicar = Math.Min(request.MontoRecibido, totalDeudasSeleccionadas);
        decimal cambio = request.MetodoPago == MetodoPago.Efectivo 
            ? Math.Max(0, request.MontoRecibido - totalDeudasSeleccionadas) 
            : 0;

        decimal montoRestantePorAplicar = montoAAplicar;
        var pagosGeneradosIds = new List<int>();
        var detallesConceptos = new List<string>();
        bool nivelActualizado = false;

        foreach (var deuda in deudasSeleccionadas)
        {
            if (montoRestantePorAplicar <= 0) break;

            decimal saldoPendiente = deuda.MontoTotal - deuda.MontoPagado;
            decimal abonoActual = Math.Min(saldoPendiente, montoRestantePorAplicar);

            deuda.RegistrarAbono(abonoActual);
            _deudaRepo.Guardar(deuda);

            var conceptoPago = MapConceptoDeudaAPago(deuda.Concepto);
            var pago = new Pago(alumno.Id, deuda.Id, abonoActual, conceptoPago, request.MetodoPago);
            _pagoRepo.Guardar(pago);

            pagosGeneradosIds.Add(pago.Id);
            detallesConceptos.Add($"{deuda.Concepto}: ${abonoActual:N2}");

            // Progresión automática de nivel si se saldó un Libro al 100%
            if (deuda.Concepto == ConceptoDeuda.Libro && deuda.EstaPagada)
            {
                var inscripciones = _inscripcionRepo.ObtenerPorAlumno(alumno.Id);
                var inscripcionActiva = inscripciones.FirstOrDefault(i => i.Estado == EstadoInscripcion.Vigente);

                if (inscripcionActiva is not null)
                {
                    var cursos = _cursoRepo.ObtenerTodos().OrderBy(c => c.Id).ToList();
                    var siguienteCurso = cursos.FirstOrDefault(c => c.Id > inscripcionActiva.CursoId);

                    if (siguienteCurso is not null)
                    {
                        inscripcionActiva.CambiarCurso(siguienteCurso.Id);
                        _inscripcionRepo.Guardar(inscripcionActiva);
                        nivelActualizado = true;
                    }
                }
            }

            montoRestantePorAplicar -= abonoActual;
        }

        // Generación y persistencia de Recibo
        int primerPagoId = pagosGeneradosIds.First();
        string folio = $"REC-{DateTime.UtcNow:yyyyMMdd}-{primerPagoId:D4}";
        string detalleCompleto = string.Join(", ", detallesConceptos);

        var recibo = new Recibo(primerPagoId, montoAAplicar, folio, detalleCompleto);
        _reciboRepo.Guardar(recibo);

        // Saldo pendiente restante del alumno
        decimal saldoPendienteRestante = _deudaRepo.ObtenerPorAlumno(alumno.Id)
            .Where(d => !d.EstaPagada)
            .Sum(d => d.MontoTotal - d.MontoPagado);

        string mensaje = nivelActualizado 
            ? "Pago registrado exitosamente. Se actualizó el nivel del alumno al liquidar el libro."
            : "Pago registrado exitosamente.";

        return new RegistrarPagoResponse(
            pagosGeneradosIds,
            montoAAplicar,
            cambio,
            saldoPendienteRestante,
            folio,
            nivelActualizado,
            mensaje);
    }

    private static ConceptoPago MapConceptoDeudaAPago(ConceptoDeuda concepto)
    {
        return concepto switch
        {
            ConceptoDeuda.Inscripcion => ConceptoPago.Inscripcion,
            ConceptoDeuda.Mensualidad => ConceptoPago.Mensualidad,
            ConceptoDeuda.Libro => ConceptoPago.Libro,
            _ => ConceptoPago.Mensualidad
        };
    }
}
