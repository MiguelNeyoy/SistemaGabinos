namespace SistemaGabinos.Application.DTOs;

public class ExpedienteAlumnoDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Curp { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string? NombreTutor { get; set; }
    public string? ParentescoTutor { get; set; }
    public string? TelefonoTutor { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool TieneBeca { get; set; }
    public decimal CostoMensualidadPactada { get; set; }
    public decimal DescuentoBecaPactada { get; set; }
    public decimal MensualidadNeta { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string CursoActual { get; set; } = string.Empty;
    public List<PagoItem> Pagos { get; set; } = new();
    public decimal TotalPendiente { get; set; }
}
