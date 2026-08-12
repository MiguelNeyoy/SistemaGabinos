// Program.cs
// Punto de entrada de la aplicación.
// Inicializa Velopack en el milisegundo 0 para gestionar instalación, desinstalación y accesos directos.
using System;
using Velopack;

namespace SistemaGabinos;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Inicializar Velopack antes de que despierte la UI de WPF
            VelopackApp.Build().Run();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Velopack Initialization Warning: {ex.Message}");
        }

        // Iniciar aplicación WPF
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
