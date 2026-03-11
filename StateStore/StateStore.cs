using JojaAutoTasks.StateStore.Handlers;
using JojaAutoTasks.StateStore.Commands;
using JojaAutoTasks.StateStore.Models;

namespace JojaAutoTasks.StateStore;

internal sealed class StateStore
{
    private readonly StateContainer _stateContainer;
    private readonly AddOrUpdateTaskCommandHandler _addOrUpdateTaskHandler;
    private readonly CompleteTaskCommandHandler _completeTaskHandler;
    private readonly UncompleteTaskCommandHandler _uncompleteTaskHandler;
    private readonly RemoveTaskCommandHandler _removeTaskHandler;
    private readonly PinTaskCommandHandler _pinTaskHandler;
    private readonly UnpinTaskCommandHandler _unpinTaskHandler;
    public event Action<TaskSnapshot>? SnapshotChanged;


    internal StateStore(AddOrUpdateTaskCommandHandler addOrUpdateTaskHandler,
        CompleteTaskCommandHandler completeTaskHandler,
        UncompleteTaskCommandHandler uncompleteTaskHandler,
        RemoveTaskCommandHandler removeTaskHandler,
        PinTaskCommandHandler pinTaskHandler,
        UnpinTaskCommandHandler unpinTaskHandler,
        StateContainer stateContainer)
    {
        _addOrUpdateTaskHandler = addOrUpdateTaskHandler;
        _completeTaskHandler = completeTaskHandler;
        _uncompleteTaskHandler = uncompleteTaskHandler;
        _removeTaskHandler = removeTaskHandler;
        _pinTaskHandler = pinTaskHandler;
        _unpinTaskHandler = unpinTaskHandler;
        _stateContainer = stateContainer;
    }

    private void Handle(IStateCommand command)
    {
        switch (command)
        {
            case AddOrUpdateTaskCommand addOrUpdateTaskCommand:
                _addOrUpdateTaskHandler.Handle(addOrUpdateTaskCommand, _stateContainer);
                break;
            case CompleteTaskCommand completeTaskCommand:
                _completeTaskHandler.Handle(completeTaskCommand, _stateContainer);
                break;
            case UncompleteTaskCommand uncompleteTaskCommand:
                _uncompleteTaskHandler.Handle(uncompleteTaskCommand, _stateContainer);
                break;
            case RemoveTaskCommand removeTaskCommand:
                _removeTaskHandler.Handle(removeTaskCommand, _stateContainer);
                break;
            case PinTaskCommand pinTaskCommand:
                _pinTaskHandler.Handle(pinTaskCommand, _stateContainer);
                break;
            case UnpinTaskCommand unpinTaskCommand:
                _unpinTaskHandler.Handle(unpinTaskCommand, _stateContainer);
                break;
            default:
                throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");
        }

        SnapshotChanged?.Invoke(SnapshotProjector.Project(_stateContainer));
    }

    public void Dispatch(IStateCommand command)
    {
        Handle(command);
    }
}
