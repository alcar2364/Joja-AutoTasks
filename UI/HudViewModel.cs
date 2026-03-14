using JojaAutoTasks.State;

namespace JojaAutoTasks.Ui;

internal sealed class HudViewModel
{
    internal event Action<string>? NotificationRequested;

    internal void OnToastReceived(ToastEvent toast)
    {
        NotificationRequested?.Invoke(toast.TaskTitle);
    }
}