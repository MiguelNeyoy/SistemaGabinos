namespace SistemaGabinos.Domain.Entities;

public class PrecioConfiguracion
{
    public int Id { get; private set; }
    public decimal CostoInscripcion { get; private set; }
    public decimal CostoMensualidad { get; private set; }
    public decimal CostoLibro { get; private set; }
    public decimal CostoExamenUbicacion { get; private set; }
    public decimal MontoDescuentoBeca { get; private set; }

    private PrecioConfiguracion() { }

    public PrecioConfiguracion(
        decimal costoInscripcion,
        decimal costoMensualidad,
        decimal costoLibro,
        decimal costoExamenUbicacion,
        decimal montoDescuentoBeca)
    {
        Id = 1;
        ValidarYAsignarPrecios(costoInscripcion, costoMensualidad, costoLibro, costoExamenUbicacion, montoDescuentoBeca);
    }

    public void CambiarPrecios(
        decimal costoInscripcion,
        decimal costoMensualidad,
        decimal costoLibro,
        decimal costoExamenUbicacion,
        decimal montoDescuentoBeca)
    {
        ValidarYAsignarPrecios(costoInscripcion, costoMensualidad, costoLibro, costoExamenUbicacion, montoDescuentoBeca);
    }

    private void ValidarYAsignarPrecios(
        decimal costoInscripcion,
        decimal costoMensualidad,
        decimal costoLibro,
        decimal costoExamenUbicacion,
        decimal montoDescuentoBeca)
    {
        if (costoInscripcion <= 0)
            throw new ArgumentException("El costo de inscripción debe ser mayor a cero.", nameof(costoInscripcion));

        if (costoMensualidad <= 0)
            throw new ArgumentException("El costo de mensualidad debe ser mayor a cero.", nameof(costoMensualidad));

        if (costoLibro <= 0)
            throw new ArgumentException("El costo del libro debe ser mayor a cero.", nameof(costoLibro));

        if (costoExamenUbicacion <= 0)
            throw new ArgumentException("El costo del examen de ubicación debe ser mayor a cero.", nameof(costoExamenUbicacion));

        if (montoDescuentoBeca <= 0)
            throw new ArgumentException("El monto de descuento de beca debe ser mayor a cero.", nameof(montoDescuentoBeca));

        CostoInscripcion = costoInscripcion;
        CostoMensualidad = costoMensualidad;
        CostoLibro = costoLibro;
        CostoExamenUbicacion = costoExamenUbicacion;
        MontoDescuentoBeca = montoDescuentoBeca;
    }
}
