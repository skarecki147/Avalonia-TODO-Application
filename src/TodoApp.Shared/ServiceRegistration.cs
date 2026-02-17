using Microsoft.Extensions.DependencyInjection;
using TodoApp.Core.Interfaces;
using TodoApp.Infrastructure.Services;
using TodoApp.Shared.ViewModels;

namespace TodoApp.Shared;

public static class ServiceRegistration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ILocalStorageService, InMemoryLocalStorageService>();
        services.AddSingleton<IAuthService, MockAuthService>();
        services.AddSingleton<ITodoService, MockTodoService>();
        services.AddSingleton<IStatisticsService, StatisticsService>();
        services.AddSingleton<INotificationService, NotificationService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TaskEditorViewModel>();
        services.AddTransient<StatisticsViewModel>();
        services.AddSingleton<MainViewModel>();
    }
}
