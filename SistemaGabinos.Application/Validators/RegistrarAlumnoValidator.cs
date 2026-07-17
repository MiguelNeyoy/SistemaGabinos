using FluentValidation;
using SistemaGabinos.Application.DTOs;

namespace SistemaGabinos.Application.Validators;

public class RegistrarAlumnoValidator : AbstractValidator<RegistrarAlumnoRequest>
{
    public RegistrarAlumnoValidator()
    {
        RuleFor(x => x.NombreCompleto).NotEmpty();
        RuleFor(x => x.Curp).NotEmpty().Length(18);
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.UtcNow);
        RuleFor(x => x.Telefono).NotEmpty().MinimumLength(10);
        RuleFor(x => x.CursoId).GreaterThan(0);
        RuleFor(x => x.MontoInicial).GreaterThan(0);
    }
}
