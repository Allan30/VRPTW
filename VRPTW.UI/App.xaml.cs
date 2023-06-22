using System;
using System.Collections.Generic;
using System.IO;
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
            e.Handled = true;
            MessageBox.Show(e.Exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            File.AppendAllLines("log.txt", new List<string> { DateTime.Now.ToString(), e.Exception.Message, e.Exception.StackTrace ?? "", "-------------------" });
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
