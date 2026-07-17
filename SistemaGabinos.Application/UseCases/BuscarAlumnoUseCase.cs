using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class BuscarAlumnoUseCase : IBuscarAlumnoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;

    public BuscarAlumnoUseCase(IAlumnoRepository alumnoRepo)
    {
        _alumnoRepo = alumnoRepo;
    }

    public BuscarAlumnoResponse? Ejecutar(BuscarAlumnoRequest request)
    {
        var alumno = request.Id.HasValue
            ? _alumnoRepo.ObtenerPorId(request.Id.Value)
            : _alumnoRepo.ObtenerPorCURP(request.Curp!);

        if (alumno is null)
            return null;

        return new BuscarAlumnoResponse(
            alumno.Id,
            alumno.NombreCompleto,
            alumno.CURP,
            alumno.FechaNacimiento,
            alumno.Telefono,
            alumno.NombreTutor,
            alumno.ParentescoTutor,
            alumno.TelefonoTutor,
            alumno.Estado.ToString());
    }
}
