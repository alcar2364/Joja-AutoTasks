using JojaAutoTasks.StateStore.Commands;

namespace JojaAutoTasks.StateStore.Handlers;

internal sealed class RemoveTaskCommandHandler : ICommandHandler<RemoveTaskCommand>
{
    public void Handle(RemoveTaskCommand command, StateContainer state)
    {
        state.Remove(command.TaskId);
    }
}
