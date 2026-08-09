using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class FinanzasViewModel : ObservableObject
{
    private readonly IObtenerReporteFinancieroUseCase _reporteUseCase;

    [ObservableProperty]
    private DateTime _fechaInicio = DateTime.Today;

    [ObservableProperty]
    private DateTime _fechaFin = DateTime.Today.AddDays(1).AddTicks(-1);

    [ObservableProperty]
    private CorteCajaDto? _corteCaja;

    [ObservableProperty]
    private decimal _totalGlobalPorCobrar;

    [ObservableProperty]
    private int _totalAlumnosDeudores;

    [ObservableProperty]
    private string _criterioBusquedaDeudor = string.Empty;

    public ObservableCollection<PagoDetalleReporteDto> PagosPeriodo { get; } = new();
    public ObservableCollection<AlumnoDeudorDto> DeudoresFiltrados { get; } = new();
    private List<AlumnoDeudorDto> _todosLosDeudores = new();

    public event Action<int>? NavegarAExpedienteSolicitado;
    public event Action<int, int?>? NavegarACobroSolicitado;

    public FinanzasViewModel(IObtenerReporteFinancieroUseCase reporteUseCase)
    {
        _reporteUseCase = reporteUseCase;
        CargarReportes();
    }

    [RelayCommand]
    public void RealizarCorteHoy()
    {
        FechaInicio = DateTime.Today;
        FechaFin = DateTime.Today.AddDays(1).AddTicks(-1);
        CargarReportes();
    }

    [RelayCommand]
    public void CargarReportes()
    {
        var reporte = _reporteUseCase.GenerarReporte(FechaInicio, FechaFin);
        CorteCaja = reporte.CorteCaja;
        TotalGlobalPorCobrar = reporte.TotalGlobalPorCobrar;
        TotalAlumnosDeudores = reporte.TotalAlumnosDeudores;

        PagosPeriodo.Clear();
        foreach (var p in reporte.CorteCaja.Pagos)
        {
            PagosPeriodo.Add(p);
        }

        _todosLosDeudores = reporte.Deudores;
        AplicarFiltroDeudores();
    }

    partial void OnCriterioBusquedaDeudorChanged(string value) => AplicarFiltroDeudores();

    private void AplicarFiltroDeudores()
    {
        DeudoresFiltrados.Clear();
        var lista = string.IsNullOrWhiteSpace(CriterioBusquedaDeudor)
            ? _todosLosDeudores
            : _todosLosDeudores.Where(d => d.NombreAlumno.Contains(CriterioBusquedaDeudor, StringComparison.OrdinalIgnoreCase) ||
                                          d.Curp.Contains(CriterioBusquedaDeudor, StringComparison.OrdinalIgnoreCase));

        foreach (var d in lista)
        {
            DeudoresFiltrados.Add(d);
        }
    }

    [RelayCommand]
    private void VerExpediente(int alumnoId)
    {
        NavegarAExpedienteSolicitado?.Invoke(alumnoId);
    }

    [RelayCommand]
    private void IrACobro(AlumnoDeudorDto? deudor)
    {
        if (deudor is null) return;
        NavegarACobroSolicitado?.Invoke(deudor.AlumnoId, null);
    }
}
