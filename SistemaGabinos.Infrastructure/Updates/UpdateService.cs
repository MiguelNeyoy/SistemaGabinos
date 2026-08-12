// UpdateService.cs
// Implementación del servicio de actualización con Velopack y GitHub Releases.
using System.Reflection;
using Velopack;
using Velopack.Sources;

namespace SistemaGabinos.Infrastructure.Updates;

public class UpdateService : IUpdateService
{
    private UpdateManager? _updateManager;
    private string _currentRepoUrl = "https://github.com/MiguelNeyoy/SistemaGabinos";

    public bool EsAplicacionInstalada
    {
        get
        {
            try
            {
                var mgr = ObtenerManager();
                return mgr.IsInstalled;
            }
            catch
            {
                return false;
            }
        }
    }

    public string ObtenerVersionActual()
    {
        try
        {
            var mgr = ObtenerManager();
            if (mgr.IsInstalled && mgr.CurrentVersion != null)
            {
                return mgr.CurrentVersion.ToString();
            }
        }
        catch
        {
            // Ignorar y recurrir al ensamblado
        }

        var version = Assembly.GetEntryAssembly()?.GetName().Version 
                      ?? Assembly.GetExecutingAssembly().GetName().Version;

        return version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
    }

    public async Task<UpdateCheckResult> ComprobarActualizacionAsync(string repoUrl = "https://github.com/MiguelNeyoy/SistemaGabinos")
    {
        _currentRepoUrl = repoUrl;
        string versionActual = ObtenerVersionActual();

        try
        {
            var mgr = ObtenerManager(repoUrl);
            var updateInfo = await mgr.CheckForUpdatesAsync();

            if (updateInfo == null)
            {
                return new UpdateCheckResult(false, versionActual, null, null);
            }

            string nuevaVersion = updateInfo.TargetFullRelease.Version.ToString();
            return new UpdateCheckResult(true, versionActual, nuevaVersion, updateInfo);
        }
        catch (Exception ex)
        {
            return new UpdateCheckResult(false, versionActual, null, null, ex.Message);
        }
    }

    public async Task<bool> DescargarActualizacionAsync(UpdateInfo updateInfo, Action<int> progressHandler)
    {
        try
        {
            var mgr = ObtenerManager();
            await mgr.DownloadUpdatesAsync(updateInfo, progress => progressHandler(progress));
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al descargar actualización: {ex.Message}");
            return false;
        }
    }

    public void AplicarActualizacionYReiniciar(UpdateInfo updateInfo)
    {
        var mgr = ObtenerManager();
        mgr.ApplyUpdatesAndRestart(updateInfo);
    }

    private UpdateManager ObtenerManager(string? repoUrl = null)
    {
        string targetUrl = repoUrl ?? _currentRepoUrl;
        if (_updateManager == null || _currentRepoUrl != targetUrl)
        {
            _currentRepoUrl = targetUrl;
            var source = new GithubSource(targetUrl, accessToken: null, prerelease: false);
            _updateManager = new UpdateManager(source);
        }
        return _updateManager;
    }
}
