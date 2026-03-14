using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;
using StardewValley;

namespace JojaAutoTasks.Ui;

internal sealed class HudHost : IDisposable
{
    private readonly HudViewModel _hudViewModel;
    private readonly IDisposable _toastToken;
    private readonly IDisposable _snapshotToken;

    internal HudHost(HudViewModel hudViewModel)
    {
        _hudViewModel = hudViewModel;
        _hudViewModel.NotificationRequested += OnNotificationRequested;
        _toastToken = UiToastSubscriptionManager.Subscribe(OnToastReceived);
        _snapshotToken = UiSnapshotSubscriptionManager.Subscribe(OnSnapshotReceived);
        // TODO Phase 8: Initialize HUD drawable here
    }

    public void Dispose()
    {
        _hudViewModel.NotificationRequested -= OnNotificationRequested;
        _toastToken.Dispose();
        _snapshotToken.Dispose();
        // TODO Phase 8: Dispose HUD drawable here
    }

    private void OnToastReceived(ToastEvent toast)
    {
        _hudViewModel.OnToastReceived(toast);
    }

    private static void OnNotificationRequested(string title)
    {
        Game1.addHUDMessage(new HUDMessage(title, HUDMessage.newQuest_type));
    }

    private static void OnSnapshotReceived(TaskSnapshot snapshot)
    {
        // TODO Phase 8: Forward snapshot to HUD drawable
    }
}