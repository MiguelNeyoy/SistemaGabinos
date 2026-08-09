using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ObtenerReporteFinancieroUseCase : IObtenerReporteFinancieroUseCase
{
    private readonly IPagoRepository _pagoRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly IAlumnoRepository _alumnoRepo;

    public ObtenerReporteFinancieroUseCase(
        IPagoRepository pagoRepo,
        IDeudaRepository deudaRepo,
        IAlumnoRepository alumnoRepo)
    {
        _pagoRepo = pagoRepo;
        _deudaRepo = deudaRepo;
        _alumnoRepo = alumnoRepo;
    }

    public ReporteFinancieroGeneralDto GenerarReporte(DateTime? inicio = null, DateTime? fin = null)
    {
        DateTime fechaInicio = inicio ?? DateTime.Today;
        DateTime fechaFin = fin ?? DateTime.Today.AddDays(1).AddTicks(-1);

        var corteCaja = GenerarCorteCaja(fechaInicio, fechaFin);
        var deudores = ObtenerDeudores();

        decimal totalGlobalPorCobrar = deudores.Sum(d => d.TotalPendiente);
        int totalAlumnosDeudores = deudores.Count;

        return new ReporteFinancieroGeneralDto(
            corteCaja,
            deudores,
            totalGlobalPorCobrar,
            totalAlumnosDeudores);
    }

    public CorteCajaDto GenerarCorteCaja(DateTime inicio, DateTime fin)
    {
        var pagos = _pagoRepo.ObtenerPorRangoFechas(inicio, fin);
        var pagosDetalle = new List<PagoDetalleReporteDto>();

        decimal totalRecaudado = 0;
        decimal totalEfectivo = 0;
        decimal totalTarjeta = 0;
        decimal totalTransferencia = 0;
        decimal totalInscripciones = 0;
        decimal totalLibros = 0;
        decimal totalMensualidades = 0;

        foreach (var pago in pagos)
        {
            var alumno = _alumnoRepo.ObtenerPorId(pago.AlumnoId);
            string nombreAlumno = alumno?.NombreCompleto ?? "Alumno Desconocido";
            string folio = TipoFolio.Pago.Formatear(pago.Id);

            pagosDetalle.Add(new PagoDetalleReporteDto(
                pago.Id,
                folio,
                pago.AlumnoId,
                nombreAlumno,
                pago.Concepto,
                pago.MetodoPago,
                pago.Monto,
                pago.Fecha));

            totalRecaudado += pago.Monto;

            switch (pago.MetodoPago)
            {
                case MetodoPago.Efectivo:
                    totalEfectivo += pago.Monto;
                    break;
                case MetodoPago.Tarjeta:
                    totalTarjeta += pago.Monto;
                    break;
                case MetodoPago.Transferencia:
                    totalTransferencia += pago.Monto;
                    break;
            }

            switch (pago.Concepto)
            {
                case ConceptoPago.Inscripcion:
                    totalInscripciones += pago.Monto;
                    break;
                case ConceptoPago.Libro:
                    totalLibros += pago.Monto;
                    break;
                case ConceptoPago.Mensualidad:
                    totalMensualidades += pago.Monto;
                    break;
            }
        }

        return new CorteCajaDto(
            inicio,
            fin,
            totalRecaudado,
            totalEfectivo,
            totalTarjeta,
            totalTransferencia,
            totalInscripciones,
            totalLibros,
            totalMensualidades,
            pagosDetalle.Count,
            pagosDetalle);
    }

    public List<AlumnoDeudorDto> ObtenerDeudores()
    {
        var deudasPendientes = _deudaRepo.ObtenerDeudasPendientesGlobales();
        var deudoresMap = deudasPendientes
            .GroupBy(d => d.AlumnoId)
            .ToList();

        var resultado = new List<AlumnoDeudorDto>();

        foreach (var grupo in deudoresMap)
        {
            var alumno = _alumnoRepo.ObtenerPorId(grupo.Key);
            if (alumno is null || alumno.Estado == EstadoAlumno.Inactivo)
                continue;

            var deudasDetalle = grupo.Select(d => new DeudaDetalleReporteDto(
                d.Concepto,
                d.MontoTotal,
                d.MontoPagado,
                d.MontoTotal - d.MontoPagado
            )).ToList();

            decimal totalPendiente = deudasDetalle.Sum(d => d.SaldoPendiente);
            if (totalPendiente <= 0) continue;

            string conceptosTexto = string.Join(", ", deudasDetalle.Select(d => d.Concepto.ToString()).Distinct());

            resultado.Add(new AlumnoDeudorDto(
                alumno.Id,
                alumno.NombreCompleto,
                alumno.CURP,
                alumno.Telefono,
                totalPendiente,
                conceptosTexto,
                deudasDetalle));
        }

        return resultado.OrderByDescending(d => d.TotalPendiente).ToList();
    }
}
