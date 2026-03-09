using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.StateStore.Commands;

internal sealed class PinTaskCommand : IStateCommand
{
    public TaskId TaskId { get; }

    internal PinTaskCommand(TaskId taskId)
    {
        // -- Guard --//
        if (taskId == default)
            throw new ArgumentException("TaskId cannot be default.", nameof(taskId));

        TaskId = taskId;
    }
}
