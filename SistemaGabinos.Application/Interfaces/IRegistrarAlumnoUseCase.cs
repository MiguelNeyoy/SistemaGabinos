using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IRegistrarAlumnoUseCase
{
    RegistrarAlumnoResponse Ejecutar(RegistrarAlumnoRequest request);
}
