// DashboardMetricsDto.cs
// Contiene las métricas generales para mostrar en el Panel de Control.
using System;
using System.Collections.Generic;

namespace SistemaGabinos.Application.DTOs;

public record TransaccionRecienteDto(
    string NombreAlumno,
    string Concepto,
    decimal Monto,
    DateTime Fecha,
    string Estado
);

public record DashboardMetricsDto(
    int MatriculasActivas,
    decimal TotalDeudasPendientes,
    int AlumnosConDeuda,
    List<TransaccionRecienteDto> TransaccionesRecientes
);
