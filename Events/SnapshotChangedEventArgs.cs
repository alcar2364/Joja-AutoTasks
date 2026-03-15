using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Events;

internal sealed class SnapshotChangedEventArgs : EventArgs
{
    internal TaskSnapshot CurrentSnapshot { get; }

    internal SnapshotChangedEventArgs(TaskSnapshot currentSnapshot)
    {
        CurrentSnapshot = currentSnapshot;
    }
}
