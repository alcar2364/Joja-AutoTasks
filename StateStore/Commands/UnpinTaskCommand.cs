using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.StateStore.Commands;

internal sealed class UnpinTaskCommand : IStateCommand
{
    public TaskId TaskId { get; }

    internal UnpinTaskCommand(TaskId taskId)
    {
        // -- Guards -- //
        if (taskId == default)
        {
            throw new ArgumentException("TaskId cannot be default.", nameof(taskId));
        }

        TaskId = taskId;
    }
}
