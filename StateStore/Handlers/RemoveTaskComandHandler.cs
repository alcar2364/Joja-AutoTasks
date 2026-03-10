using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;
using JojaAutoTasks.StateStore.Commands;
using JojaAutoTasks.StateStore.Models;

namespace JojaAutoTasks.StateStore.Handlers;

internal sealed class RemoveTaskComandHandler : ICommandHandler<RemoveTaskCommand>
{
    public void Handle(RemoveTaskCommand command, StateContainer state)
    {
        state.Remove(command.TaskId);
    }
}
