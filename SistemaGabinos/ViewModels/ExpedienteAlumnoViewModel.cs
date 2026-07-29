using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class ExpedienteAlumnoViewModel : ObservableObject
{
    private readonly IObtenerExpedienteAlumnoUseCase _expedienteUseCase;

    [ObservableProperty]
    private ExpedienteAlumnoDto? _alumno;

    [ObservableProperty]
    private decimal _totalPendiente;

    [ObservableProperty]
    private bool _esDatosPersonales = true;

    [ObservableProperty]
    private bool _esHistorialFinanciero = false;

    public ObservableCollection<PagoItem> Pagos { get; } = new();

    public ExpedienteAlumnoViewModel(IObtenerExpedienteAlumnoUseCase expedienteUseCase)
    {
        _expedienteUseCase = expedienteUseCase;
    }

    public void CargarAlumno(int alumnoId)
    {
        var expediente = _expedienteUseCase.Ejecutar(alumnoId);
        if (expediente is null)
            return;

        Alumno = expediente;
        TotalPendiente = expediente.TotalPendiente;

        Pagos.Clear();
        foreach (var pago in expediente.Pagos)
        {
            Pagos.Add(pago);
        }
    }

    partial void OnEsDatosPersonalesChanged(bool value)
    {
        if (value && EsHistorialFinanciero)
        {
            EsHistorialFinanciero = false;
        }
    }

    partial void OnEsHistorialFinancieroChanged(bool value)
    {
        if (value && EsDatosPersonales)
        {
            EsDatosPersonales = false;
        }
    }

    public event Action<int, int?>? NavegarACobroSolicitado;

    [RelayCommand]
    private void Pagar(int? deudaId = null)
    {
        if (Alumno is null) return;
        NavegarACobroSolicitado?.Invoke(Alumno.Id, deudaId);
    }

    [RelayCommand]
    private void Editar()
    {
        // Lógica para abrir edición de datos del alumno
    }
}
