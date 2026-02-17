using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Core.Enums;
using TodoApp.Core.Models;

namespace TodoApp.Shared.ViewModels;

public partial class TaskEditorViewModel : ViewModelBase
{
    private TodoItem? _editingItem;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private TodoStatus _status = TodoStatus.Todo;

    [ObservableProperty]
    private TodoPriority _priority = TodoPriority.Medium;

    [ObservableProperty]
    private DateTimeOffset? _dueDate = DateTimeOffset.Now.AddDays(7);

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _dialogTitle = "New Task";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public Action<TodoItem>? OnSave { get; set; }
    public Action? OnCancel { get; set; }

    public List<TodoStatus> StatusOptions { get; } = Enum.GetValues<TodoStatus>().ToList();
    public List<TodoPriority> PriorityOptions { get; } = Enum.GetValues<TodoPriority>().ToList();

    public void SetForNew()
    {
        _editingItem = null;
        IsEditing = false;
        DialogTitle = "New Task";
        Title = string.Empty;
        Description = string.Empty;
        Status = TodoStatus.Todo;
        Priority = TodoPriority.Medium;
        DueDate = DateTimeOffset.Now.AddDays(7);
        ErrorMessage = string.Empty;
    }

    public void SetForEdit(TodoItem item)
    {
        _editingItem = item;
        IsEditing = true;
        DialogTitle = "Edit Task";
        Title = item.Title;
        Description = item.Description;
        Status = item.Status;
        Priority = item.Priority;
        DueDate = item.DueDate.HasValue ? new DateTimeOffset(item.DueDate.Value) : null;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Title is required.";
            return;
        }

        var item = _editingItem?.Clone() ?? new TodoItem();
        item.Title = Title.Trim();
        item.Description = Description.Trim();
        item.Status = Status;
        item.Priority = Priority;
        item.DueDate = DueDate?.DateTime;

        OnSave?.Invoke(item);
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }
}
