// PrintResult.cs
// Resultado amigable de operaciones de hardware de impresión.
namespace SistemaGabinos.Infrastructure.Hardware;

public record PrintResult(
    bool Exito,
    string Mensaje,
    bool RequiereReimpresion
);
