using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed class TaskListViewModel : UiViewModelBase, IDisposable
{
    private readonly IDisposable _snapshotSubscription;

    internal TaskListViewModel()
    {
        _snapshotSubscription = UiSnapshotSubscriptionManager.Subscribe(OnSnapshotChanged);
    }

    public void Dispose()
    {
        _snapshotSubscription.Dispose();
    }

    private void OnSnapshotChanged(TaskSnapshot snapshot)
    {
        // no-op
    }
}
