using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Infrastructure.Updates;
using Velopack;

namespace SistemaGabinos.ViewModels;

public partial class ConfiguracionViewModel : ObservableObject
{
    private readonly IObtenerPreciosConfiguracionUseCase _obtenerPreciosUseCase;
    private readonly IActualizarPreciosUseCase _actualizarPreciosUseCase;
    private readonly IUpdateService _updateService;

    private UpdateInfo? _updateInfoPendiente;

    // Valores guardados actualmente en la Base de Datos (placeholders)
    [ObservableProperty]
    private decimal _costoInscripcionBD = 500;

    [ObservableProperty]
    private decimal _costoMensualidadBD = 1400;

    [ObservableProperty]
    private decimal _costoLibroBD = 350;

    [ObservableProperty]
    private decimal _costoExamenUbicacionBD = 250;

    [ObservableProperty]
    private decimal _montoDescuentoBecaBD = 400;

    // Entradas editables del usuario
    [ObservableProperty]
    private decimal? _costoInscripcion;

    [ObservableProperty]
    private decimal? _costoMensualidad;

    [ObservableProperty]
    private decimal? _costoLibro;

    [ObservableProperty]
    private decimal? _costoExamenUbicacion;

    [ObservableProperty]
    private decimal? _montoDescuentoBeca;

    [ObservableProperty]
    private string? _mensajeExito;

    [ObservableProperty]
    private string? _mensajeError;

    [ObservableProperty]
    private bool _esCargando;

    // --- PROPIEDADES DE VELOPACK UPDATES ---
    [ObservableProperty]
    private string _versionActual = "1.0.0";

    [ObservableProperty]
    private bool _hayNuevaVersion;

    [ObservableProperty]
    private string? _nuevaVersionNombre;

    [ObservableProperty]
    private int _progresoDescarga;

    [ObservableProperty]
    private bool _esDescargandoUpdate;

    [ObservableProperty]
    private string? _mensajeUpdate;

    public decimal MensualidadSinBeca => CostoMensualidad ?? CostoMensualidadBD;

    public decimal MensualidadConBeca
    {
        get
        {
            var mensualidad = CostoMensualidad ?? CostoMensualidadBD;
            var descuento = MontoDescuentoBeca ?? MontoDescuentoBecaBD;
            return Math.Max(0, mensualidad - descuento);
        }
    }

    public ConfiguracionViewModel(
        IObtenerPreciosConfiguracionUseCase obtenerPreciosUseCase,
        IActualizarPreciosUseCase actualizarPreciosUseCase,
        IUpdateService updateService)
    {
        _obtenerPreciosUseCase = obtenerPreciosUseCase;
        _actualizarPreciosUseCase = actualizarPreciosUseCase;
        _updateService = updateService;

        VersionActual = _updateService.ObtenerVersionActual();
    }

    public void CargarPrecios()
    {
        try
        {
            EsCargando = true;
            LimpiarNotificaciones();

            VersionActual = _updateService.ObtenerVersionActual();

            var dto = _obtenerPreciosUseCase.Ejecutar();
            if (dto is not null)
            {
                CostoInscripcionBD = dto.CostoInscripcion;
                CostoMensualidadBD = dto.CostoMensualidad;
                CostoLibroBD = dto.CostoLibro;
                CostoExamenUbicacionBD = dto.CostoExamenUbicacion;
                MontoDescuentoBecaBD = dto.MontoDescuentoBeca;

                CostoInscripcion = null;
                CostoMensualidad = null;
                CostoLibro = null;
                CostoExamenUbicacion = null;
                MontoDescuentoBeca = null;
            }

            NotificarTarifasCalculadas();
        }
        catch (Exception ex)
        {
            MostrarError($"Error al cargar la configuración de precios: {ex.Message}");
        }
        finally
        {
            EsCargando = false;
        }
    }

    // --- COMANDOS DE VELOPACK ACTUALIZACIÓN ---
    [RelayCommand]
    private async Task BuscarActualizacionesAsync()
    {
        MensajeUpdate = "Consultando GitHub Releases...";
        HayNuevaVersion = false;
        _updateInfoPendiente = null;

        var result = await _updateService.ComprobarActualizacionAsync();

        if (result.ErrorMensaje != null)
        {
            MensajeUpdate = $"No se pudo consultar actualizaciones: {result.ErrorMensaje}";
            return;
        }

        if (result.HayActualizacion && result.UpdateInfo != null)
        {
            _updateInfoPendiente = result.UpdateInfo;
            NuevaVersionNombre = result.NuevaVersion;
            HayNuevaVersion = true;
            MensajeUpdate = $"¡Nueva versión v{result.NuevaVersion} disponible!";
        }
        else
        {
            MensajeUpdate = "El sistema está actualizado a la versión más reciente.";
        }
    }

    [RelayCommand]
    private async Task AplicarActualizacionAsync()
    {
        if (_updateInfoPendiente == null) return;

        try
        {
            EsDescargandoUpdate = true;
            ProgresoDescarga = 0;
            MensajeUpdate = "Descargando actualización en segundo plano...";

            bool descargado = await _updateService.DescargarActualizacionAsync(
                _updateInfoPendiente, 
                progreso => ProgresoDescarga = progreso);

            if (descargado)
            {
                MensajeUpdate = "Descarga completa. Reiniciando aplicación...";
                await Task.Delay(1000);
                _updateService.AplicarActualizacionYReiniciar(_updateInfoPendiente);
            }
            else
            {
                MensajeUpdate = "Ocurrió un error al descargar los paquetes de actualización.";
            }
        }
        catch (Exception ex)
        {
            MensajeUpdate = $"Error durante la actualización: {ex.Message}";
        }
        finally
        {
            EsDescargandoUpdate = false;
        }
    }

    partial void OnCostoMensualidadChanged(decimal? value)
    {
        NotificarTarifasCalculadas();
    }

    partial void OnMontoDescuentoBecaChanged(decimal? value)
    {
        NotificarTarifasCalculadas();
    }

    private void NotificarTarifasCalculadas()
    {
        OnPropertyChanged(nameof(MensualidadSinBeca));
        OnPropertyChanged(nameof(MensualidadConBeca));
    }

    [RelayCommand]
    private void GuardarCambios()
    {
        LimpiarNotificaciones();

        try
        {
            EsCargando = true;

            var costoInscripcionFinal = CostoInscripcion ?? CostoInscripcionBD;
            var costoMensualidadFinal = CostoMensualidad ?? CostoMensualidadBD;
            var costoLibroFinal = CostoLibro ?? CostoLibroBD;
            var costoExamenFinal = CostoExamenUbicacion ?? CostoExamenUbicacionBD;
            var montoDescuentoFinal = MontoDescuentoBeca ?? MontoDescuentoBecaBD;

            var dto = new PrecioConfiguracionDto(
                costoInscripcionFinal,
                costoMensualidadFinal,
                costoLibroFinal,
                costoExamenFinal,
                montoDescuentoFinal);

            _actualizarPreciosUseCase.Ejecutar(dto);

            CostoInscripcionBD = costoInscripcionFinal;
            CostoMensualidadBD = costoMensualidadFinal;
            CostoLibroBD = costoLibroFinal;
            CostoExamenUbicacionBD = costoExamenFinal;
            MontoDescuentoBecaBD = montoDescuentoFinal;

            CostoInscripcion = null;
            CostoMensualidad = null;
            CostoLibro = null;
            CostoExamenUbicacion = null;
            MontoDescuentoBeca = null;

            NotificarTarifasCalculadas();

            MostrarExito("Configuración de costos y becas guardada correctamente.");
        }
        catch (ArgumentException ex)
        {
            MostrarError(ex.Message);
        }
        catch (FluentValidation.ValidationException vex)
        {
            var errores = string.Join("\n• ", vex.Errors);
            MostrarError($"Existen errores de validación:\n• {errores}");
        }
        catch (Exception ex)
        {
            MostrarError($"Ocurrió un error al guardar la configuración: {ex.Message}");
        }
        finally
        {
            EsCargando = false;
        }
    }

    public void LimpiarNotificaciones()
    {
        MensajeExito = null;
        MensajeError = null;
    }

    private void MostrarExito(string mensaje)
    {
        MensajeError = null;
        MensajeExito = mensaje;
    }

    private void MostrarError(string mensaje)
    {
        MensajeExito = null;
        MensajeError = mensaje;
    }
}
