using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State.DayBoundary;
using JojaAutoTasks.State.Handlers;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.State;

internal sealed class StateStore
{
    private readonly StateContainer _stateContainer = new();
    private readonly AddOrUpdateTaskCommandHandler _addOrUpdateTaskHandler = new();
    private readonly CompleteTaskCommandHandler _completeTaskHandler = new();
    private readonly UncompleteTaskCommandHandler _uncompleteTaskHandler = new();
    private readonly RemoveTaskCommandHandler _removeTaskHandler = new();
    private readonly PinTaskCommandHandler _pinTaskHandler = new();
    private readonly UnpinTaskCommandHandler _unpinTaskHandler = new();
    private readonly ManualTaskCounter _manualTaskCounter = new();

    public event Action<TaskSnapshot>? SnapshotChanged;

    private void Handle(IStateCommand command)
    {
        long priorVersion = _stateContainer.Version;

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

        if (_stateContainer.Version != priorVersion)
        {
            SnapshotChanged?.Invoke(SnapshotProjector.Project(_stateContainer));
        }
    }

    internal void OnDayStarted(DayKey newDay)
    {
        var expiredIds = ExpirationDetector.DetectExpiredIds(_stateContainer, newDay);

        if (expiredIds.Count > 0)
        {
            DayTransitionHandler.RemoveExpiredTasks(expiredIds, _stateContainer, _removeTaskHandler);
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

    // TODO: Phase 7 will implement loading
    internal void OnSaveLoaded()
    { }

    internal void OnReturnToTitle()
    {
        _stateContainer.Clear();
        _manualTaskCounter.Reset();
    }

    private TaskId IssueNextManualTaskId()
    {
        int nextId = _manualTaskCounter.IssueNextId();
        return TaskIdFactory.CreateManual(nextId);
    }
}
