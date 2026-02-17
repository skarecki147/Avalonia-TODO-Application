using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Models;

namespace TodoApp.Shared.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private Action? _onLoginSuccess;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _isLoading;

    [ObservableProperty]
    private bool _rememberMe;

    [ObservableProperty]
    private bool _showPassword;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    public void SetLoginCallback(Action onLoginSuccess)
    {
        _onLoginSuccess = onLoginSuccess;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;

        try
        {
            var credentials = new UserCredentials
            {
                Username = Username,
                Password = Password,
                RememberMe = RememberMe
            };

            var result = await _authService.LoginAsync(credentials);

            if (result.Success)
            {
                _onLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Login failed.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanLogin() => !IsLoading
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(Password);

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        ShowPassword = !ShowPassword;
    }
}
