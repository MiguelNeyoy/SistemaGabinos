// RegistrarPagoValidator.cs
// Validador de reglas para la solicitud de registro de pago.
using FluentValidation;
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Validators;

public class RegistrarPagoValidator : AbstractValidator<RegistrarPagoRequest>
{
    public RegistrarPagoValidator()
    {
        RuleFor(x => x.AlumnoId)
            .GreaterThan(0)
            .WithMessage("Debe especificar un AlumnoId válido.");

        RuleFor(x => x.DeudasSeleccionadasIds)
            .NotNull()
            .Must(list => list != null && list.Count > 0)
            .WithMessage("Debe seleccionar al menos un concepto a pagar.");

        RuleFor(x => x.MontoRecibido)
            .GreaterThan(0)
            .WithMessage("El monto recibido debe ser mayor a $0.00.");

        RuleFor(x => x.MetodoPago)
            .IsInEnum()
            .WithMessage("Seleccione un método de pago válido.");
    }
}
