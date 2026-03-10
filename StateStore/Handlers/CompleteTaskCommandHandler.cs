using TaskStatus =JojaAutoTasks.Domain.Tasks.TaskStatus;
using JojaAutoTasks.StateStore.Commands;
using JojaAutoTasks.StateStore.Models;

namespace JojaAutoTasks.StateStore.Handlers;

internal sealed class CompleteTaskCommandHandler : ICommandHandler<CompleteTaskCommand>
{
    public void Handle(CompleteTaskCommand command, StateContainer state)
    {
        if (state.TryGet(command.TaskId, out TaskRecord? existingRecord))
        {
            existingRecord.Status = TaskStatus.Completed;
            existingRecord.CompletionDay = command.CompletionDay;

            state.Set(command.TaskId, existingRecord);
        }
    }
}
