using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IObtenerExpedienteAlumnoUseCase
{
    ExpedienteAlumnoDto? Ejecutar(int alumnoId);
}
