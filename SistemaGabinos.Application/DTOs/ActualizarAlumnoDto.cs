namespace SistemaGabinos.Application.DTOs;

public record ActualizarAlumnoRequest(
    int Id,
    string NombreCompleto,
    DateTime FechaNacimiento,
    string Telefono,
    string? NombreTutor,
    string? ParentescoTutor,
    string? TelefonoTutor);
