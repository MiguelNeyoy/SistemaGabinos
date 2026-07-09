// ConceptoDeuda.cs
// Define los conceptos por los que se puede generar una deuda (cuenta por cobrar).
// - Inscripcion: cargo inicial por inscripción.
// - Mensualidad: cargo mensual recurrente.
// - Libro: cargo por libro/nivel al pasar de nivel.
namespace SistemaGabinos.Domain.Enums;

public enum ConceptoDeuda
{
    Inscripcion,
    Mensualidad,
    Libro
}