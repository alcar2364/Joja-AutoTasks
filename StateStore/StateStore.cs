using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.StateStore.DayBoundary;
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
    private readonly ManualTaskCounter _manualTaskCounter;

    public event Action<TaskSnapshot>? SnapshotChanged;


    internal StateStore(AddOrUpdateTaskCommandHandler addOrUpdateTaskHandler,
        CompleteTaskCommandHandler completeTaskHandler,
        UncompleteTaskCommandHandler uncompleteTaskHandler,
        RemoveTaskCommandHandler removeTaskHandler,
        PinTaskCommandHandler pinTaskHandler,
        UnpinTaskCommandHandler unpinTaskHandler,
        ManualTaskCounter manualTaskCounter,
        StateContainer stateContainer)
    {
        _addOrUpdateTaskHandler = addOrUpdateTaskHandler;
        _completeTaskHandler = completeTaskHandler;
        _uncompleteTaskHandler = uncompleteTaskHandler;
        _removeTaskHandler = removeTaskHandler;
        _pinTaskHandler = pinTaskHandler;
        _unpinTaskHandler = unpinTaskHandler;
        _manualTaskCounter = manualTaskCounter;
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
                throw new InvalidOperationException(
                    $"No handler found for command type {command.GetType().Name}");
        }

        SnapshotChanged?.Invoke(SnapshotProjector.Project(_stateContainer));
    }

    internal void OnDayStarted(DayKey newDay)
    {
        var expiredIds = ExpirationDetector.DetectExpiredIds(_stateContainer, newDay);

        if (expiredIds.Count > 0)
        {
            DayTransitionHandler.RemoveExpiredTasks(expiredIds, _stateContainer);
            SnapshotChanged?.Invoke(SnapshotProjector.Project(_stateContainer));
        }
    }

    internal void Dispatch(IStateCommand command)
    {
        Handle(command);
    }

    internal void DispatchCreateManualTaskCommand(
        TaskCategory category,
        string title,
        string? description,
        DayKey creationDay
        )
    {
        TaskId taskId = IssueNextManualTaskId();
        var command = new AddOrUpdateTaskCommand(
            taskId: taskId,
            category: category,
            sourceType: TaskSourceType.Manual,
            title: title,
            description: description,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: creationDay,
            sourceIdentifier: "Player"
        );
        Dispatch(command);
    }

    private TaskId IssueNextManualTaskId()
    {
        int nextId = _manualTaskCounter.IssueNextId();
        return TaskIdFactory.CreateManual(nextId);
    }
}
