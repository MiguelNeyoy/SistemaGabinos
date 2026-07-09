// DomainException.cs
// Excepción base para todos los errores de la capa de dominio.
// Proporciona un constructor que recibe un mensaje descriptivo.
namespace SistemaGabinos.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
}
