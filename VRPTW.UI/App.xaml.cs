using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;
using Serilog;
using VRPTW.UI.Views;

namespace VRPTW.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;
    
    public App()
    {
        _host = Host
            .CreateDefaultBuilder()
            .UseSerilog((host, loggerConfig) =>
                loggerConfig
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Information())
            .ConfigureServices(services => services.AddSingleton(s => new MainWindow()))
            .Build();
        
        DispatcherUnhandledException += Application_DispatcherUnhandledException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host.Start();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            e.Handled = true;
            var logger = _host.Services.GetRequiredService<ILogger>();
            logger.Fatal(e.Exception, "Unhandled exception");
            MessageBox.Show(e.Exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            logger.Fatal("Shutting down application.");
        }
        catch
        {
        }
        finally
        {
            Shutdown(-1);
        }
    }
}
