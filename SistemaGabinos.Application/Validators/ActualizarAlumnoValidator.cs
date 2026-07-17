using FluentValidation;
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Validators;

public class ActualizarAlumnoValidator : AbstractValidator<ActualizarAlumnoRequest>
{
    public ActualizarAlumnoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NombreCompleto).NotEmpty();
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.UtcNow);
        RuleFor(x => x.Telefono).NotEmpty().MinimumLength(10);
    }
}
