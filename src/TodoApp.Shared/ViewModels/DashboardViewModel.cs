using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using TodoApp.Core.Enums;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Models;

namespace TodoApp.Shared.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly ITodoService _todoService;
    private readonly INotificationService _notificationService;
    private List<TodoItem> _allItems = new();
    private TodoItem? _lastDeletedItem;

    [ObservableProperty]
    private ObservableCollection<TodoItemViewModel> _tasks = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private TodoStatus? _filterStatus;

    [ObservableProperty]
    private TodoPriority? _filterPriority;

    [ObservableProperty]
    private SortOption _sortBy = SortOption.CreatedDate;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private int _totalItems;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _showEditor;

    [ObservableProperty]
    private TaskEditorViewModel? _editorViewModel;

    [ObservableProperty]
    private bool _showEmptyState;

    [ObservableProperty]
    private bool _isSelectMode;

    [ObservableProperty]
    private bool _showUndoBar;

    [ObservableProperty]
    private string _undoMessage = string.Empty;

    private const int PageSize = 10;

    public List<TodoStatus?> StatusFilterOptions { get; } = new() { null, TodoStatus.Todo, TodoStatus.InProgress, TodoStatus.Done };
    public List<TodoPriority?> PriorityFilterOptions { get; } = new() { null, TodoPriority.Low, TodoPriority.Medium, TodoPriority.High };
    public List<SortOption> SortOptions { get; } = Enum.GetValues<SortOption>().ToList();

    public DashboardViewModel(ITodoService todoService, INotificationService notificationService)
    {
        _todoService = todoService;
        _notificationService = notificationService;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        try
        {
            await _todoService.LoadAsync();
            _allItems = await _todoService.GetAllAsync();
            ApplyFiltersAndRefresh();
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchQueryChanged(string value)
    {
        CurrentPage = 1;
        ApplyFiltersAndRefresh();
    }

    partial void OnFilterStatusChanged(TodoStatus? value)
    {
        CurrentPage = 1;
        ApplyFiltersAndRefresh();
    }

    partial void OnFilterPriorityChanged(TodoPriority? value)
    {
        CurrentPage = 1;
        ApplyFiltersAndRefresh();
    }

    partial void OnSortByChanged(SortOption value)
    {
        ApplyFiltersAndRefresh();
    }

    private void ApplyFiltersAndRefresh()
    {
        var filtered = _allItems.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var q = SearchQuery.ToLower();
            filtered = filtered.Where(i =>
                i.Title.ToLower().Contains(q) ||
                i.Description.ToLower().Contains(q));
        }

        if (FilterStatus.HasValue)
            filtered = filtered.Where(i => i.Status == FilterStatus.Value);

        if (FilterPriority.HasValue)
            filtered = filtered.Where(i => i.Priority == FilterPriority.Value);

        filtered = SortBy switch
        {
            SortOption.Priority => filtered.OrderByDescending(i => i.Priority),
            SortOption.DueDate => filtered.OrderBy(i => i.DueDate ?? DateTime.MaxValue),
            SortOption.Status => filtered.OrderBy(i => i.Status),
            SortOption.Title => filtered.OrderBy(i => i.Title),
            _ => filtered.OrderBy(i => i.OrderIndex)
        };

        var list = filtered.ToList();
        TotalItems = list.Count;
        TotalPages = Math.Max(1, (int)Math.Ceiling(list.Count / (double)PageSize));
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;

        var paged = list.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

        Tasks = new ObservableCollection<TodoItemViewModel>(
            paged.Select(item => new TodoItemViewModel(item, this)));

        ShowEmptyState = _allItems.Count == 0;
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            ApplyFiltersAndRefresh();
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            ApplyFiltersAndRefresh();
        }
    }

    [RelayCommand]
    private void AddNewTask()
    {
        var editor = new TaskEditorViewModel();
        editor.SetForNew();
        editor.OnSave = async item =>
        {
            await _todoService.AddAsync(item);
            _allItems = await _todoService.GetAllAsync();
            ShowEditor = false;
            ApplyFiltersAndRefresh();
            _notificationService.Show("Task created successfully!", NotificationType.Success);
        };
        editor.OnCancel = () => ShowEditor = false;
        EditorViewModel = editor;
        ShowEditor = true;
    }

    public void EditTask(TodoItem item)
    {
        var editor = new TaskEditorViewModel();
        editor.SetForEdit(item);
        editor.OnSave = async updated =>
        {
            await _todoService.UpdateAsync(updated);
            _allItems = await _todoService.GetAllAsync();
            ShowEditor = false;
            ApplyFiltersAndRefresh();
            _notificationService.Show("Task updated!", NotificationType.Success);
        };
        editor.OnCancel = () => ShowEditor = false;
        EditorViewModel = editor;
        ShowEditor = true;
    }

    public async Task DeleteTask(TodoItem item)
    {
        _lastDeletedItem = item.Clone();
        await _todoService.DeleteAsync(item.Id);
        _allItems = await _todoService.GetAllAsync();
        ApplyFiltersAndRefresh();

        UndoMessage = $"Deleted \"{item.Title}\"";
        ShowUndoBar = true;
        _notificationService.Show($"Task \"{item.Title}\" deleted", NotificationType.Warning);

        _ = HideUndoAfterDelay();
    }

    private async Task HideUndoAfterDelay()
    {
        await Task.Delay(5000);
        ShowUndoBar = false;
    }

    [RelayCommand]
    private async Task UndoDeleteAsync()
    {
        if (_lastDeletedItem != null)
        {
            await _todoService.AddAsync(_lastDeletedItem);
            _allItems = await _todoService.GetAllAsync();
            ApplyFiltersAndRefresh();
            ShowUndoBar = false;
            _notificationService.Show("Task restored!", NotificationType.Success);
            _lastDeletedItem = null;
        }
    }

    public async Task ToggleStatus(TodoItem item)
    {
        item.Status = item.Status switch
        {
            TodoStatus.Todo => TodoStatus.InProgress,
            TodoStatus.InProgress => TodoStatus.Done,
            TodoStatus.Done => TodoStatus.Todo,
            _ => TodoStatus.Todo
        };
        await _todoService.UpdateAsync(item);
        _allItems = await _todoService.GetAllAsync();
        ApplyFiltersAndRefresh();
    }

    [RelayCommand]
    private void ToggleSelectMode()
    {
        IsSelectMode = !IsSelectMode;
        if (!IsSelectMode)
        {
            foreach (var t in Tasks) t.IsSelected = false;
        }
    }

    [RelayCommand]
    private async Task BulkDeleteAsync()
    {
        var selected = Tasks.Where(t => t.IsSelected).Select(t => t.Item.Id).ToList();
        foreach (var id in selected)
        {
            await _todoService.DeleteAsync(id);
        }
        _allItems = await _todoService.GetAllAsync();
        IsSelectMode = false;
        ApplyFiltersAndRefresh();
        _notificationService.Show($"Deleted {selected.Count} task(s)", NotificationType.Warning);
    }

    [RelayCommand]
    private async Task BulkSetPriorityAsync(string priorityStr)
    {
        if (!Enum.TryParse<TodoPriority>(priorityStr, out var priority)) return;
        var selected = Tasks.Where(t => t.IsSelected).ToList();
        foreach (var t in selected)
        {
            t.Item.Priority = priority;
            await _todoService.UpdateAsync(t.Item);
        }
        _allItems = await _todoService.GetAllAsync();
        IsSelectMode = false;
        ApplyFiltersAndRefresh();
        _notificationService.Show($"Updated {selected.Count} task(s) to {priority} priority", NotificationType.Success);
    }

    public async Task MoveToTop(TodoItem item)
    {
        await _todoService.ReorderAsync(item.Id, 0);
        _allItems = await _todoService.GetAllAsync();
        ApplyFiltersAndRefresh();
    }

    public async Task MoveToBottom(TodoItem item)
    {
        await _todoService.ReorderAsync(item.Id, _allItems.Count - 1);
        _allItems = await _todoService.GetAllAsync();
        ApplyFiltersAndRefresh();
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchQuery = string.Empty;
        FilterStatus = null;
        FilterPriority = null;
        SortBy = SortOption.CreatedDate;
        CurrentPage = 1;
        ApplyFiltersAndRefresh();
    }
}

public partial class TodoItemViewModel : ObservableObject
{
    public TodoItem Item { get; }
    private readonly DashboardViewModel _parent;

    [ObservableProperty]
    private bool _isSelected;

    public string StatusIcon => Item.Status switch
    {
        TodoStatus.Done => "\u2714",
        TodoStatus.InProgress => "\u25B6",
        _ => "\u25CB"
    };

    public MaterialIconKind StatusIconKind => Item.Status switch
    {
        TodoStatus.Done => MaterialIconKind.CheckCircle,
        TodoStatus.InProgress => MaterialIconKind.PlayCircleOutline,
        _ => MaterialIconKind.CircleOutline
    };

    public string PriorityLabel => Item.Priority.ToString();
    public bool IsOverdue => Item.IsOverdue;

    public TodoItemViewModel(TodoItem item, DashboardViewModel parent)
    {
        Item = item;
        _parent = parent;
    }

    [RelayCommand]
    private async Task ToggleStatusAsync()
    {
        await _parent.ToggleStatus(Item);
    }

    [RelayCommand]
    private void Edit()
    {
        _parent.EditTask(Item);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await _parent.DeleteTask(Item);
    }

    [RelayCommand]
    private async Task MoveTopAsync()
    {
        await _parent.MoveToTop(Item);
    }

    [RelayCommand]
    private async Task MoveBottomAsync()
    {
        await _parent.MoveToBottom(Item);
    }
}
