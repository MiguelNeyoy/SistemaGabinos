using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IBuscarAlumnoUseCase
{
    BuscarAlumnoResponse? Ejecutar(BuscarAlumnoRequest request);
}
