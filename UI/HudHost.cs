using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;
using StardewValley;

namespace JojaAutoTasks.Ui;

internal sealed class HudHost : IDisposable
{
    private readonly HudViewModel _hudViewModel;
    private readonly IDisposable _snapshotToken;
    private readonly IDisposable _toastToken;

    internal HudHost(HudViewModel hudViewModel)
    {
        _hudViewModel = hudViewModel;
        _hudViewModel.NotificationRequested += OnNotificationRequested;
        _snapshotToken = UiSnapshotSubscriptionManager.Subscribe(OnSnapshotReceived);
        _toastToken = UiToastSubscriptionManager.Subscribe(OnToastReceived);
        // TODO Phase 8: Initialize HUD drawable here
    }

    public void Dispose()
    {
        _hudViewModel.NotificationRequested -= OnNotificationRequested;
        _snapshotToken.Dispose();
        _toastToken.Dispose();
        // TODO Phase 8: Dispose HUD drawable here
    }

    private static void OnNotificationRequested(string title)
    {
        Game1.addHUDMessage(new HUDMessage(title, HUDMessage.newQuest_type));
    }

    private static void OnSnapshotReceived(TaskSnapshot snapshot)
    {
        // TODO Phase 8: Forward snapshot to HUD drawable
    }

    private static void OnToastReceived(ToastEvent toast)
    {
        // TODO Phase 8: Forward toast to HUD drawable if needed
    }
}