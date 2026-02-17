using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Models;

namespace TodoApp.Shared.ViewModels;

public partial class StatisticsViewModel : ViewModelBase
{
    private readonly IStatisticsService _statisticsService;

    [ObservableProperty]
    private int _totalTasks;

    [ObservableProperty]
    private int _completedTasks;

    [ObservableProperty]
    private int _inProgressTasks;

    [ObservableProperty]
    private int _todoTasks;

    [ObservableProperty]
    private int _overdueTasks;

    [ObservableProperty]
    private double _completionRate;

    [ObservableProperty]
    private double _avgCompletionHours;

    [ObservableProperty]
    private int _highPriorityCount;

    [ObservableProperty]
    private int _mediumPriorityCount;

    [ObservableProperty]
    private int _lowPriorityCount;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ISeries[] _statusPieSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] _weeklyBarSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] _weeklyXAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private Axis[] _weeklyYAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private ISeries[] _priorityPieSeries = Array.Empty<ISeries>();

    public StatisticsViewModel(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var stats = await _statisticsService.GetStatisticsAsync();

            TotalTasks = stats.TotalTasks;
            CompletedTasks = stats.CompletedTasks;
            InProgressTasks = stats.InProgressTasks;
            TodoTasks = stats.TodoTasks;
            OverdueTasks = stats.OverdueTasks;
            CompletionRate = stats.CompletionRate;
            AvgCompletionHours = stats.AverageCompletionTimeHours;
            HighPriorityCount = stats.HighPriorityTasks;
            MediumPriorityCount = stats.MediumPriorityTasks;
            LowPriorityCount = stats.LowPriorityTasks;

            BuildStatusPieChart(stats);
            BuildPriorityPieChart(stats);
            BuildWeeklyBarChart(stats);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void BuildStatusPieChart(StatisticsData stats)
    {
        StatusPieSeries = new ISeries[]
        {
            new PieSeries<int>
            {
                Values = new[] { stats.CompletedTasks },
                Name = "Done",
                Fill = new SolidColorPaint(SKColor.Parse("#4caf50")),
                Stroke = null
            },
            new PieSeries<int>
            {
                Values = new[] { stats.InProgressTasks },
                Name = "In Progress",
                Fill = new SolidColorPaint(SKColor.Parse("#ff9800")),
                Stroke = null
            },
            new PieSeries<int>
            {
                Values = new[] { stats.TodoTasks },
                Name = "Todo",
                Fill = new SolidColorPaint(SKColor.Parse("#2196f3")),
                Stroke = null
            }
        };
    }

    private void BuildPriorityPieChart(StatisticsData stats)
    {
        PriorityPieSeries = new ISeries[]
        {
            new PieSeries<int>
            {
                Values = new[] { stats.HighPriorityTasks },
                Name = "High",
                Fill = new SolidColorPaint(SKColor.Parse("#f44336")),
                Stroke = null
            },
            new PieSeries<int>
            {
                Values = new[] { stats.MediumPriorityTasks },
                Name = "Medium",
                Fill = new SolidColorPaint(SKColor.Parse("#ff9800")),
                Stroke = null
            },
            new PieSeries<int>
            {
                Values = new[] { stats.LowPriorityTasks },
                Name = "Low",
                Fill = new SolidColorPaint(SKColor.Parse("#4caf50")),
                Stroke = null
            }
        };
    }

    private void BuildWeeklyBarChart(StatisticsData stats)
    {
        var labels = stats.WeeklyActivity.Keys.ToArray();
        var values = stats.WeeklyActivity.Values.ToArray();

        WeeklyBarSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = values,
                Name = "Activity",
                Fill = new SolidColorPaint(SKColor.Parse("#6c63ff")),
                Stroke = null,
                MaxBarWidth = 32,
                Rx = 4,
                Ry = 4
            }
        };

        WeeklyXAxes = new Axis[]
        {
            new Axis
            {
                Labels = labels,
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#a0aec0")),
                TextSize = 12
            }
        };

        WeeklyYAxes = new Axis[]
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#a0aec0")),
                TextSize = 12,
                MinLimit = 0
            }
        };
    }
}
