using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed class HudViewModel : UiViewModelBase, IDisposable
{
    private readonly IDisposable _snapshotSubscription;

    private int _activeTaskCount;
    public int ActiveTaskCount
    {
        get => _activeTaskCount;
        private set
        {
            if (_activeTaskCount == value)
                return;
            _activeTaskCount = value;
            OnPropertyChanged();
        }
    }

    private int _completedTaskCount;
    public int CompletedTaskCount
    {
        get => _completedTaskCount;
        private set
        {
            if (_completedTaskCount == value)
                return;
            _completedTaskCount = value;
            OnPropertyChanged();
        }
    }

    private int _pinnedTaskCount;
    public int PinnedTaskCount
    {
        get => _pinnedTaskCount;
        private set
        {
            if (_pinnedTaskCount == value)
                return;
            _pinnedTaskCount = value;
            OnPropertyChanged();
        }
    }

    private long _lastSnapshotVersion;
    public long LastSnapshotVersion
    {
        get => _lastSnapshotVersion;
        private set
        {
            if (_lastSnapshotVersion == value)
                return;
            _lastSnapshotVersion = value;
            OnPropertyChanged();
        }
    }

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
        LastSnapshotVersion = snapshot.Version;
    }
}
