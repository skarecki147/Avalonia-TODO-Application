namespace TodoApp.Core.Models;

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Username { get; set; }
}
