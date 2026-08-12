namespace SistemaGabinos.Application.DTOs;

public record AlumnoDeudorDto(
    int AlumnoId,
    string NombreAlumno,
    string Curp,
    string Telefono,
    decimal TotalPendiente,
    string ConceptosPendientesTexto,
    List<DeudaDetalleReporteDto> DeudasPendientes);
