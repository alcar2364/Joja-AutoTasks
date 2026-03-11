namespace JojaAutoTasks.StateStore.Models;

internal sealed class TaskSnapshot
{
    internal IReadOnlyList<TaskView> TaskViews { get; }
    internal int Version { get; }

    internal TaskSnapshot(IReadOnlyList<TaskView> taskViews, int version)
    {
        TaskViews = taskViews;
        Version = version;
    }
}
