// TutorObligatorioException.cs
// Se lanza cuando un alumno menor de 18 años no tiene los datos del tutor completos.
namespace SistemaGabinos.Domain.Exceptions;

public class TutorObligatorioException()
    : DomainException("El alumno es menor de edad. Los datos del tutor (NombreTutor, ParentescoTutor, TelefonoTutor) son obligatorios.")
{
}
