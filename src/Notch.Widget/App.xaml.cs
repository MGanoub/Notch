using System.Configuration;
using System.Data;
using System.Windows;
using Notch.Shared.Dto;
using Notch.Widget.Services;
using Notch.Widget.Windows;

namespace Notch.Widget;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var tokens = TokenStorage.Load();
        if (tokens is null)
        {
            new LoginWindow().ShowDialog();
            
            // re-check — did login actually succeed?
            tokens = TokenStorage.Load(); 
        }
        if (tokens is null)
        {
            Shutdown();
            return;
        }
        new MainWidgetWindow().Show();
    }
}

