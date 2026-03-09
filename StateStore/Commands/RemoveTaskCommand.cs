using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.StateStore.Commands;

internal sealed class RemoveTaskCommand : IStateCommand
{
    public TaskId TaskId { get; }

    internal RemoveTaskCommand(TaskId taskId)
    {
        // -- Guard --//
        if (taskId == default)
            throw new ArgumentException("TaskId cannot be default.", nameof(taskId));

        TaskId = taskId;
    }
}
