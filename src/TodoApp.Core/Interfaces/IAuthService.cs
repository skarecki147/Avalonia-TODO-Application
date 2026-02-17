using TodoApp.Core.Models;

namespace TodoApp.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(UserCredentials credentials);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetCurrentUserAsync();
}
