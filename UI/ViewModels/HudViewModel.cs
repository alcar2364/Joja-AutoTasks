using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed class HudViewModel : UiViewModelBase, IDisposable
{
    private readonly IDisposable _snapshotSubscription;

    internal HudViewModel()
    {
        _snapshotSubscription = UiSnapshotSubscriptionManager.Subscribe(OnSnapshotChanged);
    }

    public void Dispose()
    {
        _snapshotSubscription.Dispose();
    }

    private void OnSnapshotChanged(TaskSnapshot snapshot)
    {
        // no op
    }
}
