using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed class TaskListViewModel : UiViewModelBase
{
    private readonly IDisposable _snapshotSubscription;

    internal TaskListViewModel()
    {
        _snapshotSubscription = UiSnapshotSubscriptionManager.Subscribe(OnSnapshotChanged);
    }

    private void OnSnapshotChanged(TaskSnapshot snapshot)
    {
        // no-op
    }
}
