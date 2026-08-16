using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Entities;
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

        var config = _precioConfigRepo.Obtener();
        alumno.ActualizarCondicionesPago(alumno.CostoMensualidadPactada, config.MontoDescuentoBeca);
        _alumnoRepo.Guardar(alumno);

        RecalcularMensualidadPendiente(alumno);

        return $"Beca asignada a '{alumno.NombreCompleto}'. Mensualidad pendiente recalculada.";
    }

    public string QuitarBeca(int alumnoId)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId)
            ?? throw new KeyNotFoundException("Alumno no encontrado.");

        if (!alumno.TieneBeca)
            throw new DomainException("El alumno no tiene beca asignada.");

        alumno.ActualizarCondicionesPago(alumno.CostoMensualidadPactada, 0);
        _alumnoRepo.Guardar(alumno);

        RecalcularMensualidadPendiente(alumno);

        return $"Beca retirada de '{alumno.NombreCompleto}'. Mensualidad pendiente recalculada.";
    }

    private void RecalcularMensualidadPendiente(Alumno alumno)
    {
        var deudas = _deudaRepo.ObtenerPorAlumno(alumno.Id);
        var mensualidadPendiente = deudas
            .FirstOrDefault(d => d.Concepto == ConceptoDeuda.Mensualidad && !d.EstaPagada);

        if (mensualidadPendiente is null) return;

        mensualidadPendiente.RecalcularMonto(alumno.MensualidadNeta);
        _deudaRepo.Guardar(mensualidadPendiente);
    }
}
