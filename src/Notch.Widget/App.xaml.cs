using System.Configuration;
using System.Data;
using System.Windows;
using Notch.Shared.Dto;
using Notch.Widget.Services;

namespace Notch.Widget;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var loaded = TokenStorage.Load();
        Console.WriteLine(loaded?.AccessToken);
    }
}

