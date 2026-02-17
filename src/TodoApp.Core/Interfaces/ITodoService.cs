using TodoApp.Core.Enums;
using TodoApp.Core.Models;

namespace TodoApp.Core.Interfaces;

public interface ITodoService
{
    Task<List<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task AddAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task DeleteAsync(Guid id);
    Task ReorderAsync(Guid itemId, int newIndex);
    Task<List<TodoItem>> SearchAsync(string query);
    Task SaveAsync();
    Task LoadAsync();
}
