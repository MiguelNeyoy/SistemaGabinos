using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class ReportesFinanzasViewModel : ObservableObject
{
    private readonly IObtenerReporteFinancieroUseCase _reporteUseCase;

    [ObservableProperty]
    private DateTime _fechaInicio = DateTime.Today;

    [ObservableProperty]
    private DateTime _fechaFin = DateTime.Today;

    [ObservableProperty]
    private ReporteFinancieroGeneralDto? _reporte;

    // Action para notificar a la vista (MainWindow) que se solicitó ir a un expediente
    public event Action<int>? SolicitaIrAExpediente;

    public ReportesFinanzasViewModel(IObtenerReporteFinancieroUseCase reporteUseCase)
    {
        _reporteUseCase = reporteUseCase;
        GenerarReporte(); // Carga inicial por defecto con el día de hoy
    }

    [RelayCommand]
    private void GenerarReporte()
    {
        // El backend toma inicio de día para FechaInicio y fin de día para FechaFin por defecto
        Reporte = _reporteUseCase.GenerarReporte(FechaInicio, FechaFin);
    }

    [RelayCommand]
    private void IrAExpediente(AlumnoDeudorDto? alumno)
    {
        if (alumno != null)
        {
            SolicitaIrAExpediente?.Invoke(alumno.AlumnoId);
        }
    }

    [RelayCommand]
    private void ExportarPdf()
    {
        // TODO: Lógica futura para exportar el Reporte actual a PDF
        System.Windows.MessageBox.Show("Funcionalidad de Exportación a PDF en construcción.", "Exportar PDF", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }
}
