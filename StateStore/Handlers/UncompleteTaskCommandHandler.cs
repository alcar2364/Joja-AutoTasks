using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;
using JojaAutoTasks.StateStore.Commands;
using JojaAutoTasks.StateStore.Models;

namespace JojaAutoTasks.StateStore.Handlers;

internal sealed class UncompleteTaskCommandHandler : ICommandHandler<UncompleteTaskCommand>
{
    public void Handle(UncompleteTaskCommand command, StateContainer state)
    {
        if (state.TryGet(command.TaskId, out TaskRecord? existingRecord))
        {
            existingRecord.Status = TaskStatus.Incomplete;
            existingRecord.CompletionDay = null;

            state.Set(command.TaskId, existingRecord);
        }
    }
}

