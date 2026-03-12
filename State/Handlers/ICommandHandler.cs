using JojaAutoTasks.State.Commands;
namespace JojaAutoTasks.State.Handlers;

internal interface ICommandHandler<TCommand> where TCommand : IStateCommand
{
    void Handle(TCommand command, StateContainer state);
}
