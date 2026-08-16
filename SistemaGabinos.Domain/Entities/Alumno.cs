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
    public DateTime ProximaFechaCobro { get; private set; }
    public decimal CostoMensualidadPactada { get; private set; }
    public decimal DescuentoBecaPactada { get; private set; }

    public decimal MensualidadNeta => Math.Max(0, CostoMensualidadPactada - DescuentoBecaPactada);
    public bool TieneBeca => DescuentoBecaPactada > 0;

    private Alumno() { }

    public Alumno(
        string nombreCompleto,
        string curp,
        DateTime fechaNacimiento,
        string telefono,
        string? nombreTutor,
        string? parentescoTutor,
        string? telefonoTutor,
        decimal costoMensualidadPactada = 800m,
        decimal descuentoBecaPactada = 0m)
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

        if (string.IsNullOrWhiteSpace(telefono) || telefono.Length < 10)
        {
            throw new ArgumentException("El teléfono debe tener al menos 10 caracteres.", nameof(telefono));
        }

        NombreCompleto = nombreCompleto;
        CURP = curp.Trim().ToUpper();
        FechaNacimiento = fechaNacimiento;
        Telefono = telefono;
        NombreTutor = nombreTutor;
        ParentescoTutor = parentescoTutor;
        TelefonoTutor = telefonoTutor;
        FechaRegistro = DateTime.UtcNow;
        ProximaFechaCobro = FechaRegistro.AddMonths(1);
        Estado = EstadoAlumno.Activo;
        ActualizarCondicionesPago(costoMensualidadPactada, descuentoBecaPactada);
    }

    public void ActualizarCondicionesPago(decimal nuevoCosto, decimal nuevaBeca)
    {
        if (nuevoCosto <= 0)
            throw new ArgumentException("El costo de mensualidad debe ser mayor a $0.00.", nameof(nuevoCosto));

        if (nuevaBeca < 0)
            throw new ArgumentException("El descuento de beca no puede ser negativo.", nameof(nuevaBeca));

        if (nuevaBeca >= nuevoCosto)
            throw new ArgumentException("La beca no puede ser mayor o igual al 100% de la mensualidad.", nameof(nuevaBeca));

        CostoMensualidadPactada = nuevoCosto;
        DescuentoBecaPactada = nuevaBeca;
    }

    public void AvanzarProximaFechaCobro()
    {
        ProximaFechaCobro = ProximaFechaCobro.AddMonths(1);
    }

    private static readonly Regex CurpRegex = new(
        @"^[A-Z]{4}\d{6}[HM][A-Z]{5}[0-9A-Z]\d$",
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

    public void Reactivar()
    {
        Estado = EstadoAlumno.Activo;
    }

    public void CambiarNombre(string nombreCompleto)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto))
            throw new ArgumentException("El nombre completo no puede estar vacío.", nameof(nombreCompleto));
        NombreCompleto = nombreCompleto;
    }

    public void CambiarFechaNacimiento(DateTime fechaNacimiento)
    {
        if (fechaNacimiento > DateTime.UtcNow)
            throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(fechaNacimiento));
        FechaNacimiento = fechaNacimiento;
    }

    public void CambiarTelefono(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono) || telefono.Length < 10)
            throw new ArgumentException("El teléfono debe tener al menos 10 caracteres.", nameof(telefono));
        Telefono = telefono;
    }

    public void CambiarNombreTutor(string? nombreTutor)
    {
        NombreTutor = nombreTutor;
    }

    public void CambiarParentescoTutor(string? parentescoTutor)
    {
        ParentescoTutor = parentescoTutor;
    }

    public void CambiarTelefonoTutor(string? telefonoTutor)
    {
        TelefonoTutor = telefonoTutor;
    }

    public void ActualizarDatos(
        string nombreCompleto,
        DateTime fechaNacimiento,
        string telefono,
        string? nombreTutor,
        string? parentescoTutor,
        string? telefonoTutor)
    {
        CambiarNombre(nombreCompleto);
        CambiarFechaNacimiento(fechaNacimiento);
        CambiarTelefono(telefono);
        CambiarNombreTutor(nombreTutor);
        CambiarParentescoTutor(parentescoTutor);
        CambiarTelefonoTutor(telefonoTutor);
        ValidarReglasDeNegocio();
    }
}
