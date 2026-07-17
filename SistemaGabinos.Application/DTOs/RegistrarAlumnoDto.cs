namespace SistemaGabinos.Application.DTOs;

public record RegistrarAlumnoRequest(
    string NombreCompleto,
    string Curp,
    DateTime FechaNacimiento,
    string Telefono,
    string? NombreTutor,
    string? ParentescoTutor,
    string? TelefonoTutor,
    int CursoId,
    decimal MontoInicial);

public record RegistrarAlumnoResponse(
    int AlumnoId,
    string Mensaje);
