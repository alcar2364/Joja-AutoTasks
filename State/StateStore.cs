using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.DayBoundary;
using JojaAutoTasks.State.Handlers;
using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.State;

internal sealed class StateStore
{
    private const string ProjectionCalledBeforeTimeContextWasInitializedMessage =
        "Projection called before time context was initialized.";

    private readonly StateContainer _stateContainer = new();
    private readonly AddOrUpdateTaskCommandHandler _addOrUpdateTaskHandler = new();
    private readonly CompleteTaskCommandHandler _completeTaskHandler = new();
    private readonly UncompleteTaskCommandHandler _uncompleteTaskHandler = new();
    private readonly RemoveTaskCommandHandler _removeTaskHandler = new();
    private readonly PinTaskCommandHandler _pinTaskHandler = new();
    private readonly UnpinTaskCommandHandler _unpinTaskHandler = new();
    private readonly ManualTaskCounter _manualTaskCounter = new();
    private DayKey _currentDayKey;
    private int _currentTime;
    private bool _sessionActive;
    private bool _timeContextInitialized;
    private bool _bootstrapWarnEmitted;
    private BootstrapGuardPolicy _bootstrapGuardPolicy = BootstrapGuardPolicy.Release;
    private Action<string>? _warnAction;

    public event Action<TaskSnapshot>? SnapshotChanged;
    public event Action<ToastEvent>? ToastRequested;

    internal void Dispatch(IStateCommand command)
    {
        Handle(command);
    }

    internal void DispatchCreateManualTaskCommand(
        TaskCategory category,
        string title,
        string? description,
        DayKey creationDay)
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
            sourceIdentifier: "Player");
        Dispatch(command);
    }

    internal void SetBootstrapGuardPolicy(BootstrapGuardPolicy policy)
    {
        _bootstrapGuardPolicy = policy;
    }

    internal void SetWarnAction(Action<string> warn)
    {
        ArgumentNullException.ThrowIfNull(warn);
        _warnAction = warn;
    }

    internal void InitializeTimeContext(DayKey currentDay, int currentTime)
    {
        _currentDayKey = currentDay;
        _currentTime = currentTime;
        _timeContextInitialized = true;
    }

    internal void OnDayStarted(DayKey newDay, int currentTime)
    {
        InitializeTimeContext(newDay, currentTime);

        var expiredIds = ExpirationDetector.DetectExpiredIds(_stateContainer, newDay);

        if (expiredIds.Count > 0)
        {
            DayTransitionHandler.RemoveExpiredTasks(expiredIds, _stateContainer, _removeTaskHandler);
            PublishSnapshot();
        }

        _sessionActive = true;
    }

    internal void OnTimeChanged(DayKey currentDay, int currentTime)
    {
        if (!_sessionActive)
        {
            return;
        }

        if (_timeContextInitialized && currentDay == _currentDayKey && currentTime == _currentTime)
        {
            return;
        }

        _currentDayKey = currentDay;
        _currentTime = currentTime;
        _timeContextInitialized = true;
    }

    internal void OnSaveLoaded()
    {
        _sessionActive = true;
        _bootstrapWarnEmitted = false;
    }

    internal void OnReturnToTitle()
    {
        _sessionActive = false;
        _timeContextInitialized = false;
        _bootstrapWarnEmitted = false;
        _currentDayKey = default;
        _currentTime = default;
        _stateContainer.Clear();
        _manualTaskCounter.Reset();
    }

    private void Handle(IStateCommand command)
    {
        long priorVersion = _stateContainer.Version;

        switch (command)
        {
            case AddOrUpdateTaskCommand addOrUpdateTaskCommand:
                _addOrUpdateTaskHandler.Handle(addOrUpdateTaskCommand, _stateContainer);
                break;
            case CompleteTaskCommand completeTaskCommand:
                HandleCompleteTaskCommand(completeTaskCommand, priorVersion);
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
            PublishSnapshot();
        }
    }

    private void HandleCompleteTaskCommand(CompleteTaskCommand completeTaskCommand, long priorVersion)
    {
        TaskStatus? priorStatus = null;
        if (_stateContainer.TryGet(completeTaskCommand.TaskId, out TaskRecord? existingRecord))
        {
            priorStatus = existingRecord.Status;
        }

        _completeTaskHandler.Handle(completeTaskCommand, _stateContainer);

        if (_stateContainer.Version != priorVersion
            && priorStatus == TaskStatus.Incomplete
            && !completeTaskCommand.IsPlayerInitiated
            && _stateContainer.TryGet(completeTaskCommand.TaskId, out TaskRecord? updatedRecord))
        {
            ToastRequested?.Invoke(new ToastEvent(ToastType.TaskAutoCompleted, updatedRecord.Title));
        }
    }

    private TaskId IssueNextManualTaskId()
    {
        int nextId = _manualTaskCounter.IssueNextId();
        return TaskIdFactory.CreateManual(nextId);
    }

    private void PublishSnapshot()
    {
        if (!_timeContextInitialized)
        {
            GuardTimeContextInitialized();
            return;
        }

        SnapshotChanged?.Invoke(SnapshotProjector.Project(_stateContainer, _currentDayKey, _currentTime));
    }

    private void GuardTimeContextInitialized()
    {
        if (_timeContextInitialized)
        {
            return;
        }

        switch (_bootstrapGuardPolicy)
        {
            case BootstrapGuardPolicy.Debug:
                throw new InvalidOperationException(ProjectionCalledBeforeTimeContextWasInitializedMessage);
            case BootstrapGuardPolicy.DebugDiagnostic:
                _warnAction?.Invoke(ProjectionCalledBeforeTimeContextWasInitializedMessage);
                return;
            case BootstrapGuardPolicy.Release:
                if (!_bootstrapWarnEmitted)
                {
                    _warnAction?.Invoke(ProjectionCalledBeforeTimeContextWasInitializedMessage);
                    _bootstrapWarnEmitted = true;
                }

                return;
            default:
                return;
        }
    }
}
