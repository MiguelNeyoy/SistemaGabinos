using System.Windows;
using Microsoft.EntityFrameworkCore;
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
                    options.UseSqlite("Data Source=SistemaGabinos.db"));

                services.AddScoped<IAlumnoRepository, AlumnoRepository>();
                services.AddScoped<ICursoRepository, CursoRepository>();
                services.AddScoped<IInscripcionRepository, InscripcionRepository>();
                services.AddScoped<IDeudaRepository, DeudaRepository>();
                services.AddScoped<IPagoRepository, PagoRepository>();

                services.AddSingleton<RegistrarAlumnoValidator>();
                services.AddScoped<IRegistrarAlumnoUseCase, RegistrarAlumnoUseCase>();
                services.AddScoped<IBuscarAlumnoUseCase, BuscarAlumnoUseCase>();

                services.AddTransient<NuevaMatriculaViewModel>();
                services.AddTransient<NuevaMatricula>();
                services.AddTransient<PanelDeControl>();
            })
            .Build();

        _host.Start();

        using (var scope = _host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SistemaGabinosDBContext>();
            db.Database.Migrate();
        }

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
