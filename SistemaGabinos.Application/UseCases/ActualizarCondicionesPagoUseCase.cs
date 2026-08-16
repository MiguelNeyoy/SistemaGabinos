using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Domain.Entities;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Interfaces;

namespace SistemaGabinos.Application.UseCases;

public class ActualizarCondicionesPagoUseCase : IActualizarCondicionesPagoUseCase
{
    private readonly IAlumnoRepository _alumnoRepo;
    private readonly IDeudaRepository _deudaRepo;

    public ActualizarCondicionesPagoUseCase(
        IAlumnoRepository alumnoRepo,
        IDeudaRepository deudaRepo)
    {
        _alumnoRepo = alumnoRepo;
        _deudaRepo = deudaRepo;
    }

    public string Ejecutar(int alumnoId, decimal nuevoCosto, decimal nuevaBeca)
    {
        var alumno = _alumnoRepo.ObtenerPorId(alumnoId)
            ?? throw new KeyNotFoundException($"No se encontró al alumno con ID {alumnoId}.");

        alumno.ActualizarCondicionesPago(nuevoCosto, nuevaBeca);
        _alumnoRepo.Guardar(alumno);

        // Recalcular cualquier deuda de mensualidad pendiente
        var deudas = _deudaRepo.ObtenerPorAlumno(alumno.Id);
        var mensualidadPendiente = deudas
            .FirstOrDefault(d => d.Concepto == ConceptoDeuda.Mensualidad && !d.EstaPagada);

        if (mensualidadPendiente is not null)
        {
            mensualidadPendiente.RecalcularMonto(alumno.MensualidadNeta);
            _deudaRepo.Guardar(mensualidadPendiente);
        }

        return $"Condiciones de pago actualizadas para '{alumno.NombreCompleto}'. Mensualidad Neta: ${alumno.MensualidadNeta:N2}.";
    }
}
