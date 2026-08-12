namespace SistemaGabinos.Application.DTOs;

public record BuscarAlumnoRequest(
    int? Id,
    string? Curp);

public record BuscarAlumnoResponse(
    int Id,
    string NombreCompleto,
    string Curp,
    DateTime FechaNacimiento,
    string Telefono,
    string? NombreTutor,
    string? ParentescoTutor,
    string? TelefonoTutor,
    string Estado);
