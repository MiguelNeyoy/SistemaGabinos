using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Infrastructure.Hardware;

namespace SistemaGabinos.ViewModels;

public partial class ReportesFinanzasViewModel : ObservableObject
{
    private readonly IObtenerReporteFinancieroUseCase _reporteUseCase;
    private readonly IPdfRenderService _pdfRenderService;
    private readonly IExcelExportService _excelExportService;

    [ObservableProperty]
    private DateTime _fechaInicio = DateTime.Today;

    [ObservableProperty]
    private DateTime _fechaFin = DateTime.Today;

    [ObservableProperty]
    private ReporteFinancieroGeneralDto? _reporte;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    // Action para notificar a la vista (MainWindow) que se solicitó ir a un expediente
    public event Action<int>? SolicitaIrAExpediente;

    public ReportesFinanzasViewModel(
        IObtenerReporteFinancieroUseCase reporteUseCase,
        IPdfRenderService pdfRenderService,
        IExcelExportService excelExportService)
    {
        _reporteUseCase = reporteUseCase;
        _pdfRenderService = pdfRenderService;
        _excelExportService = excelExportService;
        GenerarReporte();
    }

    [RelayCommand]
    private void GenerarReporte()
    {
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
        if (Reporte?.CorteCaja is null) return;

        var corte = Reporte.CorteCaja;
        var items = corte.Pagos.Select(p => new TicketItemData($"{p.Concepto} - {p.NombreAlumno}", p.Monto)).ToList();
        if (items.Count == 0)
        {
            items.Add(new TicketItemData("Sin transacciones en el periodo", 0));
        }

        var ticketData = new TicketData(
            $"CORTE DE CAJA {corte.FechaInicio:dd/MM/yyyy} - {corte.FechaFin:dd/MM/yyyy}",
            $"Efectivo: ${corte.TotalEfectivo:N2} | Tarjeta: ${corte.TotalTarjeta:N2} | Transferencia: ${corte.TotalTransferencia:N2}",
            items,
            corte.TotalRecaudado,
            corte.TotalRecaudado,
            0,
            Domain.Enums.MetodoPago.Efectivo,
            DateTime.Now,
            $"CORTE-{DateTime.Now:yyyyMMdd}"
        );

        byte[] pdfBytes = _pdfRenderService.RenderizarReciboPdf(ticketData);

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"CorteCaja_{corte.FechaInicio:yyyyMMdd}_{corte.FechaFin:yyyyMMdd}.pdf",
            DefaultExt = ".pdf",
            Filter = "Archivos PDF (.pdf)|*.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllBytes(dialog.FileName, pdfBytes);
            Mensaje = $"Reporte PDF exportado: {System.IO.Path.GetFileName(dialog.FileName)}";
        }
    }

    [RelayCommand]
    private void ExportarCsv()
    {
        if (Reporte?.CorteCaja is null) return;

        byte[] csvBytes = _excelExportService.GenerarCsvCorteCaja(Reporte.CorteCaja);

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"CorteCaja_{Reporte.CorteCaja.FechaInicio:yyyyMMdd}_{Reporte.CorteCaja.FechaFin:yyyyMMdd}.csv",
            DefaultExt = ".csv",
            Filter = "Archivos CSV (.csv)|*.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllBytes(dialog.FileName, csvBytes);
            Mensaje = $"Reporte CSV exportado: {System.IO.Path.GetFileName(dialog.FileName)}";
        }
    }

    [RelayCommand]
    private void ExportarDeudoresCsv()
    {
        if (Reporte is null) return;

        byte[] csvBytes = _excelExportService.GenerarCsvDeudores(Reporte.Deudores, Reporte.TotalGlobalPorCobrar);

        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"Deudores_{DateTime.Now:yyyyMMdd}.csv",
            DefaultExt = ".csv",
            Filter = "Archivos CSV (.csv)|*.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllBytes(dialog.FileName, csvBytes);
            Mensaje = $"Reporte de deudores exportado: {System.IO.Path.GetFileName(dialog.FileName)}";
        }
    }
}
