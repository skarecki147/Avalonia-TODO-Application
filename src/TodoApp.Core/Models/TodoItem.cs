using TodoApp.Core.Enums;

namespace TodoApp.Core.Models;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TodoStatus Status { get; set; } = TodoStatus.Todo;
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int OrderIndex { get; set; }

    public bool IsOverdue => DueDate.HasValue
        && DueDate.Value.Date < DateTime.UtcNow.Date
        && Status != TodoStatus.Done;

    public TodoItem Clone() => new()
    {
        Id = Id,
        Title = Title,
        Description = Description,
        Status = Status,
        Priority = Priority,
        DueDate = DueDate,
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt,
        OrderIndex = OrderIndex
    };
}
