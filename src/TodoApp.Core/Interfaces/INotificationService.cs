namespace TodoApp.Core.Interfaces;

public interface INotificationService
{
    event Action<string, NotificationType>? OnNotification;
    void Show(string message, NotificationType type = NotificationType.Info);
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}
