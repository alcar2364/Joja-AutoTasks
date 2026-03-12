using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.State.Commands;

internal sealed class PinTaskCommand : IStateCommand
{
    public TaskId TaskId { get; }

    internal PinTaskCommand(TaskId taskId)
    {
        // -- Guards -- //
        if (taskId == default)
        {
            throw new ArgumentException("TaskId cannot be default.", nameof(taskId));
        }

        TaskId = taskId;
    }
}
