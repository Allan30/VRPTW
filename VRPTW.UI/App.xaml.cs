using System.Windows;
using System.Windows.Threading;

namespace VRPTW.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += Application_DispatcherUnhandledException;
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            MessageBox.Show(e.Exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Shutdown(-1);
        }
        catch
        {
            Shutdown(-1);
        }
        
    }
}
