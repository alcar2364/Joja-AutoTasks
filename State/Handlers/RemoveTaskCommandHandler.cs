using JojaAutoTasks.State.Commands;

namespace JojaAutoTasks.State.Handlers;

internal sealed class RemoveTaskCommandHandler : ICommandHandler<RemoveTaskCommand>
{
    public void Handle(RemoveTaskCommand command, StateContainer state)
    {
        state.Remove(command.TaskId);
    }
}
