using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.State.Commands;

/// <summary>
/// Command to mark a task as uncompleted.
/// </summary>
internal sealed class UncompleteTaskCommand : IStateCommand
{
    public TaskId TaskId { get; }

    internal UncompleteTaskCommand(TaskId taskId)
    {
        // -- Guards -- //
        if (taskId == default)
        {
            throw new ArgumentException("taskId cannot be the default value.", nameof(taskId));
        }

        TaskId = taskId;
    }
}
