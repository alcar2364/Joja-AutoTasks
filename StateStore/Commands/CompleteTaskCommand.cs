using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.StateStore.Commands;

/// <summary>
/// Represents a command to mark a task as completed in the state store.
/// </summary>
internal sealed class CompleteTaskCommand : IStateCommand
{
    public TaskId TaskId { get; }

    public DayKey CompletionDay { get; }

    internal CompleteTaskCommand(TaskId taskId, DayKey completionDay)
    {
        if (taskId == default)
        {
            throw new ArgumentException("taskId cannot be the default value.", nameof(taskId));
        }

        if (completionDay == default)
        {
            throw new ArgumentException("completionDay cannot be the default value.", nameof(completionDay));
        }

        TaskId = taskId;
        CompletionDay = completionDay;
    }
}
