namespace JojaAutoTasks.State.Models;

internal sealed class TaskSnapshot
{
    internal IReadOnlyList<TaskView> TaskViews { get; }
    internal long Version { get; }

    internal TaskSnapshot(IReadOnlyList<TaskView> taskViews, long version)
    {
        TaskViews = taskViews;
        Version = version;
    }
}
