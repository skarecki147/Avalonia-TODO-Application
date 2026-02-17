namespace TodoApp.Core.Models;

public class StatisticsData
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int TodoTasks { get; set; }
    public int OverdueTasks { get; set; }
    public double CompletionRate { get; set; }
    public double AverageCompletionTimeHours { get; set; }
    public int HighPriorityTasks { get; set; }
    public int MediumPriorityTasks { get; set; }
    public int LowPriorityTasks { get; set; }
    public Dictionary<string, int> WeeklyActivity { get; set; } = new();
}
