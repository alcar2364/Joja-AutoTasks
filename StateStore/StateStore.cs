using JojaAutoTasks.StateStore.Handlers;

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


    internal StateStore(AddOrUpdateTaskCommandHandler addOrUpdateTaskHandler,
        CompleteTaskCommandHandler completeTaskHandler,
        UncompleteTaskCommandHandler uncompleteTaskHandler,
        RemoveTaskCommandHandler removeTaskHandler,
        PinTaskCommandHandler pinTaskHandler,
        UnpinTaskCommandHandler unpinTaskHandler,
        StateContainer stateContainer)
    {
        
    }
}
