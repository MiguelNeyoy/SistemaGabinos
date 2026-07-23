using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class BuscarAlumnosSugerenciasUseCase : IBuscarAlumnosSugerenciasUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;

    public BuscarAlumnosSugerenciasUseCase(IAlumnoRepository alumnoRepo)
    {
        _alumnoRepo = alumnoRepo;
    }

    public List<AlumnoSugerenciaDto> Ejecutar(string criterio, int maxResultados = 10)
    {
        if (string.IsNullOrWhiteSpace(criterio))
            return new List<AlumnoSugerenciaDto>();

        var alumnos = _alumnoRepo.BuscarPorNombreOCurp(criterio, maxResultados);

        return alumnos
            .Select(a => new AlumnoSugerenciaDto(a.Id, a.NombreCompleto, a.CURP))
            .ToList();
    }
}
