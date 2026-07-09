// Alumno.cs
// Entidad principal que representa un estudiante registrado en el sistema.
// Si el alumno es menor de 18 años, los datos del tutor (NombreTutor, ParentescoTutor, TelefonoTutor) son obligatorios.
// CURP es un identificador único mexicano.
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

    public void ValidarReglasDeNegocio()
    {
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
