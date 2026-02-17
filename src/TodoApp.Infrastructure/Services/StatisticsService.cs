using TodoApp.Core.Enums;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Models;

namespace TodoApp.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    private readonly ITodoService _todoService;

    public StatisticsService(ITodoService todoService)
    {
        _todoService = todoService;
    }

    public async Task<StatisticsData> GetStatisticsAsync()
    {
        var items = await _todoService.GetAllAsync();

        var total = items.Count;
        var completed = items.Count(i => i.Status == TodoStatus.Done);
        var inProgress = items.Count(i => i.Status == TodoStatus.InProgress);
        var todo = items.Count(i => i.Status == TodoStatus.Todo);
        var overdue = items.Count(i => i.IsOverdue);

        var completionRate = total > 0 ? (double)completed / total * 100 : 0;

        var completedItems = items.Where(i => i.Status == TodoStatus.Done).ToList();
        var avgCompletionHours = completedItems.Count > 0
            ? completedItems.Average(i => (i.UpdatedAt - i.CreatedAt).TotalHours)
            : 0;

        var weeklyActivity = new Dictionary<string, int>();
        var today = DateTime.UtcNow.Date;
        for (int i = 6; i >= 0; i--)
        {
            var day = today.AddDays(-i);
            var dayName = day.ToString("ddd");
            var count = items.Count(item =>
                item.CreatedAt.Date == day || item.UpdatedAt.Date == day);
            weeklyActivity[dayName] = count;
        }

        return new StatisticsData
        {
            TotalTasks = total,
            CompletedTasks = completed,
            InProgressTasks = inProgress,
            TodoTasks = todo,
            OverdueTasks = overdue,
            CompletionRate = Math.Round(completionRate, 1),
            AverageCompletionTimeHours = Math.Round(avgCompletionHours, 1),
            HighPriorityTasks = items.Count(i => i.Priority == TodoPriority.High),
            MediumPriorityTasks = items.Count(i => i.Priority == TodoPriority.Medium),
            LowPriorityTasks = items.Count(i => i.Priority == TodoPriority.Low),
            WeeklyActivity = weeklyActivity
        };
    }
}
