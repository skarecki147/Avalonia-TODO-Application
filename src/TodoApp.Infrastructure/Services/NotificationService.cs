using TodoApp.Core.Interfaces;

namespace TodoApp.Infrastructure.Services;

public class NotificationService : INotificationService
{
    public event Action<string, NotificationType>? OnNotification;

    public void Show(string message, NotificationType type = NotificationType.Info)
    {
        OnNotification?.Invoke(message, type);
    }
}
