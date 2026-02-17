using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Core.Interfaces;

namespace TodoApp.Shared.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly INotificationService _notificationService;
    private readonly ILocalStorageService _storageService;
    private readonly IServiceProvider _serviceProvider;

    private const string ThemeKey = "app_theme";

    [ObservableProperty]
    private ViewModelBase? _currentView;

    [ObservableProperty]
    private bool _isAuthenticated;

    [ObservableProperty]
    private string _currentUser = string.Empty;

    public string CurrentUserInitial =>
        string.IsNullOrEmpty(CurrentUser) ? "U" : char.ToUpperInvariant(CurrentUser[0]).ToString();

    partial void OnCurrentUserChanged(string value) => OnPropertyChanged(nameof(CurrentUserInitial));

    [ObservableProperty]
    private string _selectedNav = "Dashboard";

    [ObservableProperty]
    private string _notificationMessage = string.Empty;

    [ObservableProperty]
    private bool _showNotification;

    [ObservableProperty]
    private string _notificationColor = "#4caf50";

    [ObservableProperty]
    private bool _isDarkTheme = true;

    [ObservableProperty]
    private string _themeIcon = "\u263E"; // ☾ (kept for compatibility, use ThemeIconKind in UI)

    public MainViewModel(
        IAuthService authService,
        INotificationService notificationService,
        ILocalStorageService storageService,
        IServiceProvider serviceProvider)
    {
        _authService = authService;
        _notificationService = notificationService;
        _storageService = storageService;
        _serviceProvider = serviceProvider;

        _notificationService.OnNotification += OnNotificationReceived;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadThemePreference();

        var authenticated = await _authService.IsAuthenticatedAsync();
        if (authenticated)
        {
            CurrentUser = await _authService.GetCurrentUserAsync() ?? "User";
            IsAuthenticated = true;
            await NavigateToDashboardAsync();
        }
        else
        {
            ShowLogin();
        }
    }

    private async Task LoadThemePreference()
    {
        var theme = await _storageService.GetItemAsync(ThemeKey);
        IsDarkTheme = theme != "light";
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = IsDarkTheme
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        }
        ThemeIcon = IsDarkTheme ? "\u263E" : "\u2600"; // ☾ or ☀
        OnPropertyChanged(nameof(ThemeIconKind));
    }

    public MaterialIconKind ThemeIconKind => IsDarkTheme ? MaterialIconKind.WeatherNight : MaterialIconKind.WeatherSunny;

    partial void OnIsDarkThemeChanged(bool value) => OnPropertyChanged(nameof(ThemeIconKind));

    [RelayCommand]
    private async Task ToggleThemeAsync()
    {
        IsDarkTheme = !IsDarkTheme;
        ApplyTheme();
        await _storageService.SetItemAsync(ThemeKey, IsDarkTheme ? "dark" : "light");
    }

    private void ShowLogin()
    {
        var loginVm = _serviceProvider.GetRequiredService<LoginViewModel>();
        loginVm.SetLoginCallback(async () =>
        {
            CurrentUser = await _authService.GetCurrentUserAsync() ?? "User";
            IsAuthenticated = true;
            await NavigateToDashboardAsync();
        });
        CurrentView = loginVm;
    }

    [RelayCommand]
    private async Task NavigateToDashboardAsync()
    {
        SelectedNav = "Dashboard";
        var vm = _serviceProvider.GetRequiredService<DashboardViewModel>();
        await vm.InitializeAsync();
        CurrentView = vm;
    }

    [RelayCommand]
    private async Task NavigateToStatisticsAsync()
    {
        SelectedNav = "Statistics";
        var vm = _serviceProvider.GetRequiredService<StatisticsViewModel>();
        await vm.LoadAsync();
        CurrentView = vm;
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        IsAuthenticated = false;
        CurrentUser = string.Empty;
        ShowLogin();
    }

    public void HandleKeyboardShortcut(string key, bool ctrl)
    {
        if (ctrl && key == "N" && IsAuthenticated && CurrentView is DashboardViewModel dashboard)
        {
            dashboard.AddNewTaskCommand.Execute(null);
        }
    }

    private async void OnNotificationReceived(string message, NotificationType type)
    {
        NotificationColor = type switch
        {
            NotificationType.Success => "#4caf50",
            NotificationType.Error => "#f44336",
            NotificationType.Warning => "#ff9800",
            _ => "#2196f3"
        };
        NotificationMessage = message;
        ShowNotification = true;

        await Task.Delay(3000);
        ShowNotification = false;
    }
}
