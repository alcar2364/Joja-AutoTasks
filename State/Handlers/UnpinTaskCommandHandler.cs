
using JojaAutoTasks.State.Commands;

namespace JojaAutoTasks.State.Handlers;

internal sealed class UnpinTaskCommandHandler : ICommandHandler<UnpinTaskCommand>
{
    public void Handle(UnpinTaskCommand command, StateContainer state)
    {
        if (state.TryGet(command.TaskId, out var existingRecord))
        {
            existingRecord.IsPinned = false;
            state.Set(command.TaskId, existingRecord);
        }
    }
}
