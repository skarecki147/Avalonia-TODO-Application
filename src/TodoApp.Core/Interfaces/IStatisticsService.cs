using TodoApp.Core.Models;

namespace TodoApp.Core.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsData> GetStatisticsAsync();
}
