using JojaAutoTasks.StateStore.Commands;
namespace JojaAutoTasks.StateStore.Handlers;

internal interface ICommandHandler<TCommand> where TCommand : IStateCommand
{
    void Handle(TCommand command, StateContainer state);
}
