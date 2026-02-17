using TodoApp.Core.Interfaces;

namespace TodoApp.Infrastructure.Services;

/// <summary>
/// In-memory implementation of ILocalStorageService.
/// Used as a fallback when browser localStorage is not available.
/// The Browser project overrides this with JS interop.
/// </summary>
public class InMemoryLocalStorageService : ILocalStorageService
{
    private readonly Dictionary<string, string> _store = new();

    public Task<string?> GetItemAsync(string key)
    {
        _store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task SetItemAsync(string key, string value)
    {
        _store[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(string key)
    {
        _store.Remove(key);
        return Task.CompletedTask;
    }
}
