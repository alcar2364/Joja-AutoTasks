using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed class HudViewModel : UiViewModelBase
{
    private readonly IDisposable _snapshotSubscription;

    internal HudViewModel()
    {
        _snapshotSubscription = UiSnapshotSubscriptionManager.Subscribe(OnSnapshotChanged);
    }

    private void OnSnapshotChanged(TaskSnapshot snapshot)
    {
        // no op
    }
}
