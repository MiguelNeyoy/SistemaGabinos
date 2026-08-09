using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaGabinos.Application.Interfaces;
using SistemaGabinos.Application.UseCases;
using SistemaGabinos.Application.Validators;
using SistemaGabinos.Domain.Interfaces;
using SistemaGabinos.Infrastructure.DataBase.Context;
using SistemaGabinos.Infrastructure.DataBase.Repositories;
using SistemaGabinos.ViewModels;
using SistemaGabinos.Views;

namespace SistemaGabinos;

public partial class App : System.Windows.Application
{
    private IHost _host = null!;

    public static IServiceProvider Services => ((App)Current)._host.Services;

    protected override void OnStartup(StartupEventArgs e)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddDbContext<SistemaGabinosDBContext>(options =>
                    options.UseSqlite("Data Source=SistemaGabinos.db")
                           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

                services.AddScoped<IAlumnoRepository, AlumnoRepository>();
                services.AddScoped<ICursoRepository, CursoRepository>();
                services.AddScoped<IInscripcionRepository, InscripcionRepository>();
                services.AddScoped<IDeudaRepository, DeudaRepository>();
                services.AddScoped<IPagoRepository, PagoRepository>();
                services.AddScoped<IReciboRepository, ReciboRepository>();
                services.AddScoped<IPrecioConfiguracionRepository, PrecioConfiguracionRepository>();
                services.AddSingleton<SistemaGabinos.Infrastructure.Hardware.IPdfRenderService, SistemaGabinos.Infrastructure.Hardware.PdfRenderService>();
                services.AddSingleton<SistemaGabinos.Infrastructure.Hardware.IPrinterService, SistemaGabinos.Infrastructure.Hardware.PrinterService>();
                services.AddSingleton<SistemaGabinos.Infrastructure.Hardware.ITicketPrinter, SistemaGabinos.Infrastructure.Hardware.TicketPrinter>();

                services.AddSingleton<RegistrarAlumnoValidator>();
                services.AddSingleton<RegistrarPagoValidator>();
                services.AddSingleton<ActualizarAlumnoValidator>();
                services.AddScoped<IRegistrarAlumnoUseCase, RegistrarAlumnoUseCase>();
                services.AddScoped<IRegistrarPagoUseCase, RegistrarPagoUseCase>();
                services.AddScoped<IBuscarAlumnoUseCase, BuscarAlumnoUseCase>();
                services.AddScoped<IBuscarAlumnosSugerenciasUseCase, BuscarAlumnosSugerenciasUseCase>();
                services.AddScoped<IObtenerExpedienteAlumnoUseCase, ObtenerExpedienteAlumnoUseCase>();
                services.AddScoped<IObtenerPreciosConfiguracionUseCase, ObtenerPreciosConfiguracionUseCase>();
                services.AddScoped<IActualizarPreciosUseCase, ActualizarPreciosUseCase>();
                services.AddScoped<IGenerarMensualidadesAniversarioUseCase, GenerarMensualidadesAniversarioUseCase>();
                services.AddScoped<ICambiarEstadoAlumnoUseCase, CambiarEstadoAlumnoUseCase>();
                services.AddScoped<IGestionarBecaUseCase, GestionarBecaUseCase>();
                services.AddScoped<IActualizarAlumnoUseCase, ActualizarAlumnoUseCase>();
                services.AddScoped<IObtenerReporteFinancieroUseCase, ObtenerReporteFinancieroUseCase>();

                services.AddTransient<MainWindowViewModel>();
                services.AddTransient<NuevaMatriculaViewModel>();
                services.AddTransient<ExpedienteAlumnoViewModel>();
                services.AddTransient<CobroExpresViewModel>();
                services.AddTransient<ConfiguracionViewModel>();
                services.AddTransient<EditarAlumnoViewModel>();
                services.AddTransient<ReportesFinanzasViewModel>();

                services.AddTransient<NuevaMatricula>();
                services.AddTransient<ExpedienteAlumno>();
                services.AddTransient<PanelDeControl>();
                services.AddTransient<Configuracion>();
                services.AddTransient<EditarAlumnoModal>();
                services.AddTransient<VentanillaCobro>();
                services.AddTransient<ReportesFinanzas>();
            })
            .Build();

        _host.Start();

        using (var scope = _host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SistemaGabinosDBContext>();
            db.Database.Migrate();

            // Startup Check F3: Generación por Aniversario de Inscripción
            var generarMensualidadesUseCase = scope.ServiceProvider.GetRequiredService<IGenerarMensualidadesAniversarioUseCase>();
            generarMensualidadesUseCase.Ejecutar();
        }

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
