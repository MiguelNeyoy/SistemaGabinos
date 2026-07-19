using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaGabinos.Models;

public class ConceptoCobroItem : INotifyPropertyChanged
{
    private string _descripcion = string.Empty;
    private decimal _monto;
    private bool _seleccionado = true;

    public string Descripcion
    {
        get => _descripcion;
        set { _descripcion = value; OnPropertyChanged(); OnPropertyChanged(nameof(NombreVisual)); }
    }

    public string NombreVisual => Descripcion;

    public decimal Monto
    {
        get => _monto;
        set { _monto = value; OnPropertyChanged(); }
    }

    public bool Seleccionado
    {
        get => _seleccionado;
        set { _seleccionado = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
