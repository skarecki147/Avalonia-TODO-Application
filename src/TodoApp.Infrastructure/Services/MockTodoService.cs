using System.Text.Json;
using TodoApp.Core.Enums;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Models;

namespace TodoApp.Infrastructure.Services;

public class MockTodoService : ITodoService
{
    private const string StorageKey = "todo_items";
    private readonly ILocalStorageService _storage;
    private List<TodoItem> _items = new();
    private bool _loaded;

    public MockTodoService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task LoadAsync()
    {
        if (_loaded) return;

        var json = await _storage.GetItemAsync(StorageKey);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                _items = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new();
            }
            catch
            {
                _items = new();
            }
        }

        if (_items.Count == 0)
        {
            SeedData();
            await SaveAsync();
        }

        _loaded = true;
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        await LoadAsync();
        return _items.OrderBy(i => i.OrderIndex).ToList();
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        await LoadAsync();
        return _items.FirstOrDefault(i => i.Id == id);
    }

    public async Task AddAsync(TodoItem item)
    {
        await LoadAsync();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        item.OrderIndex = _items.Count > 0 ? _items.Max(i => i.OrderIndex) + 1 : 0;
        _items.Add(item);
        await SaveAsync();
    }

    public async Task UpdateAsync(TodoItem item)
    {
        await LoadAsync();
        var existing = _items.FirstOrDefault(i => i.Id == item.Id);
        if (existing != null)
        {
            existing.Title = item.Title;
            existing.Description = item.Description;
            existing.Status = item.Status;
            existing.Priority = item.Priority;
            existing.DueDate = item.DueDate;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.OrderIndex = item.OrderIndex;
            await SaveAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await LoadAsync();
        _items.RemoveAll(i => i.Id == id);
        await SaveAsync();
    }

    public async Task ReorderAsync(Guid itemId, int newIndex)
    {
        await LoadAsync();
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return;

        _items.Remove(item);
        if (newIndex >= _items.Count)
            _items.Add(item);
        else
            _items.Insert(newIndex, item);

        for (int i = 0; i < _items.Count; i++)
            _items[i].OrderIndex = i;

        await SaveAsync();
    }

    public async Task<List<TodoItem>> SearchAsync(string query)
    {
        await LoadAsync();
        if (string.IsNullOrWhiteSpace(query)) return await GetAllAsync();

        var lower = query.ToLower();
        return _items
            .Where(i => i.Title.ToLower().Contains(lower) ||
                         i.Description.ToLower().Contains(lower))
            .OrderBy(i => i.OrderIndex)
            .ToList();
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_items);
        await _storage.SetItemAsync(StorageKey, json);
    }

    private void SeedData()
    {
        _items = new List<TodoItem>
        {
            new()
            {
                Title = "Set up project architecture",
                Description = "Create the multi-layer solution with Core, Infrastructure, and Shared projects.",
                Status = TodoStatus.Done,
                Priority = TodoPriority.High,
                DueDate = DateTime.UtcNow.AddDays(-2),
                OrderIndex = 0
            },
            new()
            {
                Title = "Implement authentication",
                Description = "Build login/logout flow with session persistence and validation.",
                Status = TodoStatus.InProgress,
                Priority = TodoPriority.High,
                DueDate = DateTime.UtcNow.AddDays(1),
                OrderIndex = 1
            },
            new()
            {
                Title = "Design dashboard layout",
                Description = "Create the main task management view with sidebar navigation.",
                Status = TodoStatus.InProgress,
                Priority = TodoPriority.Medium,
                DueDate = DateTime.UtcNow.AddDays(3),
                OrderIndex = 2
            },
            new()
            {
                Title = "Add statistics charts",
                Description = "Integrate LiveCharts2 for pie and bar chart visualizations.",
                Status = TodoStatus.Todo,
                Priority = TodoPriority.Medium,
                DueDate = DateTime.UtcNow.AddDays(7),
                OrderIndex = 3
            },
            new()
            {
                Title = "Write unit tests",
                Description = "Create tests for ViewModels and services using xUnit.",
                Status = TodoStatus.Todo,
                Priority = TodoPriority.Low,
                DueDate = DateTime.UtcNow.AddDays(14),
                OrderIndex = 4
            },
            new()
            {
                Title = "Deploy to Vercel",
                Description = "Configure vercel.json and publish the WASM application.",
                Status = TodoStatus.Todo,
                Priority = TodoPriority.High,
                DueDate = DateTime.UtcNow.AddDays(5),
                OrderIndex = 5
            },
            new()
            {
                Title = "Code review session",
                Description = "Review PR for authentication module with the team.",
                Status = TodoStatus.Todo,
                Priority = TodoPriority.Low,
                DueDate = DateTime.UtcNow.AddDays(-1),
                OrderIndex = 6
            }
        };
    }
}
