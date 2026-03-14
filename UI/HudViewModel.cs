using JojaAutoTasks.State;

namespace JojaAutoTasks.Ui;

internal sealed class HudViewModel : IDisposable
{
    internal event Action<string>? NotificationRequested;

    internal void OnToastReceived(ToastEvent toast)
    {
        NotificationRequested?.Invoke(toast.TaskTitle);
    }

    public void Dispose()
    {
        // Unsubscribe from all events to prevent memory leaks
        NotificationRequested = null;
    }
    
}