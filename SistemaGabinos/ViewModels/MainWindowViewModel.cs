using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaGabinos.Application.DTOs;
using SistemaGabinos.Application.Interfaces;

namespace SistemaGabinos.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IBuscarAlumnosSugerenciasUseCase _buscarSugerenciasUseCase;

    public event Action<int>? NavegarAExpedienteSolicitado;

    [ObservableProperty]
    private string _criterioBusqueda = string.Empty;

    [ObservableProperty]
    private AlumnoSugerenciaDto? _alumnoSeleccionado;

    [ObservableProperty]
    private bool _dropdownAbierto;

    public ObservableCollection<AlumnoSugerenciaDto> ResultadosBusqueda { get; } = new();

    public MainWindowViewModel(IBuscarAlumnosSugerenciasUseCase buscarSugerenciasUseCase)
    {
        _buscarSugerenciasUseCase = buscarSugerenciasUseCase;
    }

    partial void OnCriterioBusquedaChanged(string value)
    {
        ResultadosBusqueda.Clear();

        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2)
        {
            DropdownAbierto = false;
            return;
        }

        var sugerencias = _buscarSugerenciasUseCase.Ejecutar(value.Trim());
        foreach (var sug in sugerencias)
        {
            ResultadosBusqueda.Add(sug);
        }

        DropdownAbierto = ResultadosBusqueda.Count > 0;
    }

    partial void OnAlumnoSeleccionadoChanged(AlumnoSugerenciaDto? value)
    {
        if (value is not null)
        {
            DropdownAbierto = false;
            NavegarAExpedienteSolicitado?.Invoke(value.Id);
        }
    }

    [RelayCommand]
    private void SearchAccepted()
    {
        if (AlumnoSeleccionado is not null)
        {
            DropdownAbierto = false;
            NavegarAExpedienteSolicitado?.Invoke(AlumnoSeleccionado.Id);
        }
        else if (ResultadosBusqueda.Count > 0)
        {
            DropdownAbierto = false;
            var primerResultado = ResultadosBusqueda[0];
            NavegarAExpedienteSolicitado?.Invoke(primerResultado.Id);
        }
    }
}
