using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaGabinos.Models;

public class ConceptoCobroItem : INotifyPropertyChanged
{
    private int _deudaId;
    private string _descripcion = string.Empty;
    private decimal _monto;
    private bool _seleccionado = true;

    public int DeudaId
    {
        get => _deudaId;
        set { _deudaId = value; OnPropertyChanged(); }
    }

    public string Descripcion
    {
        get => _descripcion;
        set { _descripcion = value; OnPropertyChanged(); }
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
