// Alumno.cs
// Entidad principal que representa un estudiante registrado en el sistema.
// Si el alumno es menor de 18 años, los datos del tutor (NombreTutor, ParentescoTutor, TelefonoTutor) son obligatorios.
// CURP es un identificador único mexicano de 18 caracteres alfanuméricos.
using System.Text.RegularExpressions;
using SistemaGabinos.Domain.Enums;
using SistemaGabinos.Domain.Exceptions;

namespace SistemaGabinos.Domain.Entities;

public class Alumno
{
    public int Id { get; private set; }
    public string NombreCompleto { get; private set; }
    public string CURP { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public string Telefono { get; private set; }
    public string? NombreTutor { get; private set; }
    public string? ParentescoTutor { get; private set; }
    public string? TelefonoTutor { get; private set; }
    public DateTime FechaRegistro { get; private set; }
    public EstadoAlumno Estado { get; private set; }

    private Alumno() { }

    public Alumno(
        string nombreCompleto,
        string curp,
        DateTime fechaNacimiento,
        string telefono,
        string? nombreTutor,
        string? parentescoTutor,
        string? telefonoTutor)
    {

        if (string.IsNullOrWhiteSpace(nombreCompleto))
        {
            throw new ArgumentException("El nombre completo no puede estar vacío.", nameof(nombreCompleto));
        }

        if (string.IsNullOrWhiteSpace(curp) || curp.Trim().Length != 18)
        {
            throw new ArgumentException("El CURP no puede estar vacío y debe tener 18 caracteres.", nameof(curp));
        }

        if (fechaNacimiento > DateTime.UtcNow)
        {
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            throw new ArgumentException("El teléfono no puede estar vacío.", nameof(telefono));
        }

        NombreCompleto = nombreCompleto;
        CURP = curp;
        FechaNacimiento = fechaNacimiento;
        Telefono = telefono;
        NombreTutor = nombreTutor;
        ParentescoTutor = parentescoTutor;
        TelefonoTutor = telefonoTutor;
        FechaRegistro = DateTime.UtcNow;
        Estado = EstadoAlumno.Activo;
    }

    private static readonly Regex CurpRegex = new(
        @"^[A-Z]{4}\d{6}[H,M][A-Z]{5}[0-9A-Z]\d$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public void ValidarReglasDeNegocio()
    {
        if (!CurpRegex.IsMatch(CURP))
            throw new CURPInvalidoException(CURP);

        var edad = DateTime.UtcNow.Year - FechaNacimiento.Year;
        if (FechaNacimiento.Date > DateTime.UtcNow.AddYears(-edad))
            edad--;

        if (edad < 18)
        {
            if (string.IsNullOrWhiteSpace(NombreTutor) ||
                string.IsNullOrWhiteSpace(ParentescoTutor) ||
                string.IsNullOrWhiteSpace(TelefonoTutor))
            {
                throw new TutorObligatorioException();
            }
        }
    }

    public void DarDeBaja()
    {
        Estado = EstadoAlumno.Inactivo;
    }
}
