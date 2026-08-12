// IUpdateService.cs
// Interfaz para el servicio de actualizaciones automáticas con Velopack.
using Velopack;

namespace SistemaGabinos.Infrastructure.Updates;

public record UpdateCheckResult(
    bool HayActualizacion,
    string VersionActual,
    string? NuevaVersion,
    UpdateInfo? UpdateInfo,
    string? ErrorMensaje = null
);

public interface IUpdateService
{
    /// <summary>
    /// Indica si la aplicación está ejecutándose instalada mediante Velopack.
    /// </summary>
    bool EsAplicacionInstalada { get; }

    /// <summary>
    /// Obtiene la versión actual instalada de la aplicación.
    /// </summary>
    string ObtenerVersionActual();

    /// <summary>
    /// Consulta el repositorio de GitHub para verificar si existe una versión más reciente.
    /// </summary>
    Task<UpdateCheckResult> ComprobarActualizacionAsync(string repoUrl = "https://github.com/MiguelNeyoy/SistemaGabinos");

    /// <summary>
    /// Descarga los archivos de actualización del repositorio.
    /// </summary>
    Task<bool> DescargarActualizacionAsync(UpdateInfo updateInfo, Action<int> progressHandler);

    /// <summary>
    /// Aplica la actualización y reinicia la aplicación inmediatamente.
    /// </summary>
    void AplicarActualizacionYReiniciar(UpdateInfo updateInfo);
}
