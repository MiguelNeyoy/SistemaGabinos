// EstadoInscripcion.cs
// Define el estado de una inscripción.
// - Vigente: la inscripción está activa.
// - Vencida: la inscripción ha expirado.
// - Cancelada: la inscripción fue cancelada.
namespace SistemaGabinos.Domain.Enums;

public enum EstadoInscripcion
{
    Vigente,
    Vencida,
    Cancelada
}