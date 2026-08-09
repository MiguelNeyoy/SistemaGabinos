using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Application.DTOs;

public record PagoDetalleReporteDto(
    int PagoId,
    string Folio,
    int AlumnoId,
    string NombreAlumno,
    ConceptoPago Concepto,
    MetodoPago MetodoPago,
    decimal Monto,
    DateTime Fecha);
