
using JojaAutoTasks.StateStore.Commands;

namespace JojaAutoTasks.StateStore.Handlers;

internal sealed class PinTaskCommandHandler : ICommandHandler<PinTaskCommand>
{
    public void Handle(PinTaskCommand command, StateContainer state)
    {
        if (state.TryGet(command.TaskId, out var existingRecord))
        {
            existingRecord.IsPinned = true;
            state.Set(command.TaskId, existingRecord);
        }
    }
}
