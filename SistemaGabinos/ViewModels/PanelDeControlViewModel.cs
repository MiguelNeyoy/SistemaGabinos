// PanelDeControlViewModel.cs
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class PanelDeControlViewModel : ObservableObject
{
    private readonly IObtenerMetricasDashboardUseCase _obtenerMetricasUseCase;

    [ObservableProperty]
    private int _matriculasActivas;

    [ObservableProperty]
    private decimal _totalPendiente;

    [ObservableProperty]
    private int _alumnosPendientes;

    [ObservableProperty]
    private ObservableCollection<TransaccionRecienteDto> _transacciones = new();

    [ObservableProperty]
    private bool _esCargando;

    [ObservableProperty]
    private string _saludo = "Hola, Administrador";

    public PanelDeControlViewModel(IObtenerMetricasDashboardUseCase obtenerMetricasUseCase)
    {
        _obtenerMetricasUseCase = obtenerMetricasUseCase;
    }

    public void CargarMetricas()
    {
        try
        {
            EsCargando = true;
            ActualizarSaludo();

            var metricas = _obtenerMetricasUseCase.Ejecutar();
            
            MatriculasActivas = metricas.MatriculasActivas;
            TotalPendiente = metricas.TotalDeudasPendientes;
            AlumnosPendientes = metricas.AlumnosConDeuda;
            
            Transacciones.Clear();
            foreach (var t in metricas.TransaccionesRecientes)
            {
                Transacciones.Add(t);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al cargar métricas del dashboard: {ex.Message}");
        }
        finally
        {
            EsCargando = false;
        }
    }

    private void ActualizarSaludo()
    {
        var hora = DateTime.Now.Hour;
        if (hora >= 5 && hora < 12) Saludo = "Buenos días, Administrador ☀️";
        else if (hora >= 12 && hora < 19) Saludo = "Buenas tardes, Administrador 🌤️";
        else Saludo = "Buenas noches, Administrador 🌙";
    }
}
