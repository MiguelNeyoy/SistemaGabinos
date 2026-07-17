using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IActualizarAlumnoUseCase
{
    void Ejecutar(ActualizarAlumnoRequest request);
}
