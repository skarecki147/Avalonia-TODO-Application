using System.Text.Json;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Models;

namespace TodoApp.Infrastructure.Services;

public class MockAuthService : IAuthService
{
    private const string TokenKey = "auth_token";
    private const string UserKey = "auth_user";

    private readonly ILocalStorageService _storage;
    private bool _isAuthenticated;
    private string? _currentUser;

    public MockAuthService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task<AuthResult> LoginAsync(UserCredentials credentials)
    {
        await Task.Delay(600); // Simulate network latency

        if (string.IsNullOrWhiteSpace(credentials.Username))
        {
            return new AuthResult { Success = false, ErrorMessage = "Username is required." };
        }

        if (string.IsNullOrWhiteSpace(credentials.Password))
        {
            return new AuthResult { Success = false, ErrorMessage = "Password is required." };
        }

        if (credentials.Password.Length < 8)
        {
            return new AuthResult { Success = false, ErrorMessage = "Password must be at least 8 characters." };
        }

        if (!credentials.Password.Any(char.IsUpper))
        {
            return new AuthResult { Success = false, ErrorMessage = "Password must contain an uppercase letter." };
        }

        if (!credentials.Password.Any(char.IsDigit))
        {
            return new AuthResult { Success = false, ErrorMessage = "Password must contain a number." };
        }

        if (!credentials.Password.Any(c => !char.IsLetterOrDigit(c)))
        {
            return new AuthResult { Success = false, ErrorMessage = "Password must contain a special character." };
        }

        var token = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{credentials.Username}:{DateTime.UtcNow.Ticks}"));

        _isAuthenticated = true;
        _currentUser = credentials.Username;

        if (credentials.RememberMe)
        {
            await _storage.SetItemAsync(TokenKey, token);
            await _storage.SetItemAsync(UserKey, credentials.Username);
        }

        return new AuthResult
        {
            Success = true,
            Token = token,
            Username = credentials.Username
        };
    }

    public async Task LogoutAsync()
    {
        _isAuthenticated = false;
        _currentUser = null;
        await _storage.RemoveItemAsync(TokenKey);
        await _storage.RemoveItemAsync(UserKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (_isAuthenticated) return true;

        var token = await _storage.GetItemAsync(TokenKey);
        if (!string.IsNullOrEmpty(token))
        {
            _isAuthenticated = true;
            _currentUser = await _storage.GetItemAsync(UserKey);
            return true;
        }

        return false;
    }

    public async Task<string?> GetCurrentUserAsync()
    {
        if (_currentUser != null) return _currentUser;
        return await _storage.GetItemAsync(UserKey);
    }
}
