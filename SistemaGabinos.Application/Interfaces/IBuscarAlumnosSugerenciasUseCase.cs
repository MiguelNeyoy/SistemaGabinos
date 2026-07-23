using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Interfaces;

public interface IBuscarAlumnosSugerenciasUseCase
{
    List<AlumnoSugerenciaDto> Ejecutar(string criterio, int maxResultados = 10);
}
