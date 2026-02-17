using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AvaloniaApp.ViewModels;

public partial class LoginWindowViewModel : ViewModelBase
{
    private readonly Action? _onLoginSuccess;

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

    public LoginWindowViewModel(Action? onLoginSuccess = null)
    {
        _onLoginSuccess = onLoginSuccess;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async System.Threading.Tasks.Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;

        try
        {
            await System.Threading.Tasks.Task.Delay(400);

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Please enter your username.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter your password.";
                return;
            }

            _onLoginSuccess?.Invoke();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanLogin() => !IsLoading;
}
