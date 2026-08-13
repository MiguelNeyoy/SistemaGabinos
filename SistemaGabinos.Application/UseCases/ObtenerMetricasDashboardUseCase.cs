// ObtenerMetricasDashboardUseCase.cs
using System.Linq;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ObtenerMetricasDashboardUseCase : IObtenerMetricasDashboardUseCase
{
    private readonly IAlumnoRepository _alumnoRepository;
    private readonly IDeudaRepository _deudaRepository;
    private readonly IPagoRepository _pagoRepository;

    public ObtenerMetricasDashboardUseCase(
        IAlumnoRepository alumnoRepository,
        IDeudaRepository deudaRepository,
        IPagoRepository pagoRepository)
    {
        _alumnoRepository = alumnoRepository;
        _deudaRepository = deudaRepository;
        _pagoRepository = pagoRepository;
    }

    public DashboardMetricsDto Ejecutar()
    {
        // 1. Matrículas Activas
        var alumnosActivos = _alumnoRepository.ObtenerTodos()
            .Count(a => a.Estado == SistemaGabinos.Domain.Enums.EstadoAlumno.Activo);

        // 2. Deudas Pendientes Globales
        var deudasPendientes = _deudaRepository.ObtenerDeudasPendientesGlobales();
        
        decimal totalDeudasPendientes = deudasPendientes.Sum(d => d.MontoTotal - d.MontoPagado);
        int alumnosConDeuda = deudasPendientes.Select(d => d.AlumnoId).Distinct().Count();

        // 3. Transacciones Recientes (últimas 10)
        var pagosRecientes = _pagoRepository.ObtenerTransaccionesRecientes(10);
        
        var transaccionesDto = pagosRecientes.Select(p => {
            var alumno = _alumnoRepository.ObtenerPorId(p.AlumnoId);
            return new TransaccionRecienteDto(
                NombreAlumno: alumno != null ? alumno.NombreCompleto : "Desconocido",
                Concepto: p.Concepto.ToString(),
                Monto: p.Monto,
                Fecha: p.Fecha,
                Estado: p.EstaCancelado ? "Cancelado" : "Completado"
            );
        }).ToList();

        return new DashboardMetricsDto(
            MatriculasActivas: alumnosActivos,
            TotalDeudasPendientes: totalDeudasPendientes,
            AlumnosConDeuda: alumnosConDeuda,
            TransaccionesRecientes: transaccionesDto
        );
    }
}
