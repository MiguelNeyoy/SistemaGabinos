using FluentValidation;
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Validators;

public class ActualizarAlumnoValidator : AbstractValidator<ActualizarAlumnoRequest>
{
    public ActualizarAlumnoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("El ID del alumno no es válido.");
        RuleFor(x => x.NombreCompleto).NotEmpty().WithMessage("El nombre completo es obligatorio.");
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.UtcNow)
            .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");
        RuleFor(x => x.Telefono).NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 dígitos.");
    }
}
