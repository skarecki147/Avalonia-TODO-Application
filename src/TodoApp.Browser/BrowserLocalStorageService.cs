using System.Runtime.InteropServices.JavaScript;
using TodoApp.Core.Interfaces;

namespace TodoApp.Browser;

public partial class BrowserLocalStorageService : ILocalStorageService
{
    public Task<string?> GetItemAsync(string key)
    {
        try
        {
            var value = GetItem(key);
            return Task.FromResult<string?>(value);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }

    public Task SetItemAsync(string key, string value)
    {
        try
        {
            SetItem(key, value);
        }
        catch
        {
        }
        return Task.CompletedTask;
    }

    public Task RemoveItemAsync(string key)
    {
        try
        {
            RemoveItem(key);
        }
        catch
        {
        }
        return Task.CompletedTask;
    }

    [JSImport("globalThis.localStorage.getItem")]
    private static partial string? GetItem(string key);

    [JSImport("globalThis.localStorage.setItem")]
    private static partial void SetItem(string key, string value);

    [JSImport("globalThis.localStorage.removeItem")]
    private static partial void RemoveItem(string key);
}
