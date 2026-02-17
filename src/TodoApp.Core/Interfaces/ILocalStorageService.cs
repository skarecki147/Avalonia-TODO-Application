namespace TodoApp.Core.Interfaces;

public interface ILocalStorageService
{
    Task<string?> GetItemAsync(string key);
    Task SetItemAsync(string key, string value);
    Task RemoveItemAsync(string key);
}
