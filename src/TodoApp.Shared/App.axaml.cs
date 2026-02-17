using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TodoApp.Shared.ViewModels;
using TodoApp.Shared.Views;

namespace TodoApp.Shared;

public partial class App : Application
{
    public static ServiceProvider? Services { get; private set; }

    /// <summary>
    /// Optional callback to override default service registrations.
    /// Set this before initialization from the platform-specific Program.cs.
    /// </summary>
    public static Action<IServiceCollection>? ConfigureOverrides { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DisableAvaloniaDataAnnotationValidation();

        var services = new ServiceCollection();
        ServiceRegistration.ConfigureServices(services);

        ConfigureOverrides?.Invoke(services);

        Services = services.BuildServiceProvider();

        var mainVm = Services.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView
            {
                DataContext = mainVm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
