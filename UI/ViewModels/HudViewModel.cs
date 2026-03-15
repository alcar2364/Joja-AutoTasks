using CommunityToolkit.Mvvm.ComponentModel;
using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed partial class HudViewModel : UiViewModelBase, IDisposable
{
    private readonly IDisposable _snapshotSubscription;

    [ObservableProperty]
    private int _activeTaskCount;

    [ObservableProperty]
    private int _completedTaskCount;

    [ObservableProperty]
    private int _pinnedTaskCount;

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
        int activeTaskCount = snapshot.TaskViews.Count(t => t.Status == TaskStatus.Incomplete);
        int completedTaskCount = snapshot.TaskViews.Count(t => t.Status == TaskStatus.Completed);
        int pinnedTaskCount = snapshot.TaskViews.Count(t => t.IsPinned);

        ActiveTaskCount = activeTaskCount;
        CompletedTaskCount = completedTaskCount;
        PinnedTaskCount = pinnedTaskCount;
    }
}
