using JojaAutoTasks.State;

namespace JojaAutoTasks.Ui;

internal sealed class HudViewModel : IDisposable
{
    private readonly IDisposable _toastToken;

    internal event Action<string>? NotificationRequested;

    internal HudViewModel()
    {
        _toastToken = UiToastSubscriptionManager.Subscribe(OnToastReceived);
    }

    public void Dispose()
    {
        _toastToken.Dispose();
    }

    private void OnToastReceived(ToastEvent toast)
    {
        NotificationRequested?.Invoke(toast.TaskTitle);
    }
}