// GenerarMensualidadesAniversarioUseCase.cs
// Evaluación atómica e idempotente de mensualidades por aniversario de inscripción (Startup Check).
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class GenerarMensualidadesAniversarioUseCase : IGenerarMensualidadesAniversarioUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;

    public GenerarMensualidadesAniversarioUseCase(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
    }

    public int Ejecutar()
    {
        var fechaCorte = DateTime.Today;
        var alumnosEnCorte = _alumnoRepo.ObtenerAlumnosEnFechaDeCobro(fechaCorte);

        if (alumnosEnCorte.Count == 0)
            return 0;

        int deudasGeneradas = 0;

        foreach (var alumno in alumnosEnCorte)
        {
            var deudasAlumno = _deudaRepo.ObtenerPorAlumno(alumno.Id);

            // Verificación de idempotencia: No generar una nueva mensualidad si ya existe una pendiente
            bool yaTieneMensualidadPendiente = deudasAlumno
                .Any(d => d.Concepto == ConceptoDeuda.Mensualidad && !d.EstaPagada);

            if (yaTieneMensualidadPendiente)
            {
                continue;
            }

            var nuevaDeuda = new Deuda(alumno.Id, ConceptoDeuda.Mensualidad, alumno.MensualidadNeta);
            _deudaRepo.Guardar(nuevaDeuda);

            alumno.AvanzarProximaFechaCobro();
            _alumnoRepo.Guardar(alumno);

            deudasGeneradas++;
        }

        return deudasGeneradas;
    }
}
