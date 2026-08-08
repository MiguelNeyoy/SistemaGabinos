using SistemaGabinos.Domain.Enums;

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
    Horario Horario = Horario.Mañana,
    bool TieneBeca = false);

public record RegistrarAlumnoResponse(
    int AlumnoId,
    string Mensaje);
