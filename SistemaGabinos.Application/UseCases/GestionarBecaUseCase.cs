using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class GestionarBecaUseCase : IGestionarBecaUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;
    private readonly IPrecioConfiguracionRepository _precioConfigRepo;

    public GestionarBecaUseCase(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo,
        IPrecioConfiguracionRepository precioConfigRepo)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
        _precioConfigRepo = precioConfigRepo;
    }

    public string AsignarBeca(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId)
            ?? throw new KeyNotFoundException("Alumno no encontrado.");

        if (alumno.TieneBeca)
            throw new DomainException("El alumno ya tiene beca asignada.");

        alumno.AsignarBeca();
        _alumnoRepo.Guardar(alumno);

        RecalcularMensualidadPendiente(alumnoId, conBeca: true);

        return $"Beca asignada a '{alumno.NombreCompleto}'. Mensualidad pendiente recalculada.";
    }

    public string QuitarBeca(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId)
            ?? throw new KeyNotFoundException("Alumno no encontrado.");

        if (!alumno.TieneBeca)
            throw new DomainException("El alumno no tiene beca asignada.");

        alumno.QuitarBeca();
        _alumnoRepo.Guardar(alumno);

        RecalcularMensualidadPendiente(alumnoId, conBeca: false);

        return $"Beca retirada de '{alumno.NombreCompleto}'. Mensualidad pendiente recalculada.";
    }

    private void RecalcularMensualidadPendiente(int alumnoId, bool conBeca)
    {
        var config = _precioConfigRepo.Obtener();
        var deudas = _deudaRepo.ObtenerPorAlumno(alumnoId);
        var mensualidadPendiente = deudas
            .FirstOrDefault(d => d.Concepto == ConceptoDeuda.Mensualidad && !d.EstaPagada);

        if (mensualidadPendiente is null) return;

        decimal nuevoMonto = conBeca
            ? Math.Max(0, config.CostoMensualidad - config.MontoDescuentoBeca)
            : config.CostoMensualidad;

        mensualidadPendiente.RecalcularMonto(nuevoMonto);
        _deudaRepo.Guardar(mensualidadPendiente);
    }
}
