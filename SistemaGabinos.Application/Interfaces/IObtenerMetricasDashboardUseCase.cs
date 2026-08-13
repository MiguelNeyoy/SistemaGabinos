// IObtenerMetricasDashboardUseCase.cs
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IObtenerMetricasDashboardUseCase
{
    DashboardMetricsDto Ejecutar();
}
