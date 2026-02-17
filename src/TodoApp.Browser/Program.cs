using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Browser;
using TodoApp.Core.Interfaces;
using TodoApp.Shared;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        App.ConfigureOverrides = services =>
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ILocalStorageService));
            if (descriptor != null) services.Remove(descriptor);
            services.AddSingleton<ILocalStorageService, BrowserLocalStorageService>();
        };

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
