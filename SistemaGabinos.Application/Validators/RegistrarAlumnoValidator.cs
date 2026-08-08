using FluentValidation;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Domain.Enums;

namespace SistemaGabinos.Application.Validators;

public class RegistrarAlumnoValidator : AbstractValidator<RegistrarAlumnoRequest>
{
    public RegistrarAlumnoValidator()
    {
        RuleFor(x => x.NombreCompleto).NotEmpty().WithMessage("El nombre completo es obligatorio.");
        RuleFor(x => x.Curp).NotEmpty().WithMessage("La CURP es obligatoria.")
            .Length(18).WithMessage("La CURP debe tener exactamente 18 caracteres.");
        RuleFor(x => x.FechaNacimiento).LessThan(DateTime.UtcNow)
            .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");
        RuleFor(x => x.Telefono).NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MinimumLength(10).WithMessage("El teléfono debe tener al menos 10 dígitos.");
        RuleFor(x => x.CursoId).GreaterThan(0).WithMessage("Debe seleccionar un curso.");
        RuleFor(x => x.Horario).IsInEnum().WithMessage("Seleccione un horario válido.");
    }
}
