using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.State;

namespace JojaAutoTasks.Lifecycle;

/// <summary>Coordinates lifecycle logging and dispatcher forwarding.</summary>
internal sealed class LifecycleCoordinator
{
    // -- Dependencies -- //
    private readonly ModLogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly StateStore _stateStore;
    private bool _sessionActive;

    // -- Constructor -- //
    internal LifecycleCoordinator(ModLogger logger, IEventDispatcher eventDispatcher, StateStore stateStore)
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _stateStore = stateStore;
        _stateStore.SetWarnAction(message => _logger.Warn(LogEvents.StateBootstrapGuard, message));
    }

    // -- Public API -- //
    internal void HandleGameLaunched()
    {
        _logger.Debug(LogEvents.LifecycleGameLaunched, "Lifecycle event: Game launched");
        _eventDispatcher.DispatchGameLaunched();
    }

    internal void HandleSaveLoaded(DayKey currentDay, int currentTime)
    {
        _logger.Debug(LogEvents.LifecycleSaveLoaded, "Lifecycle event: Save loaded");
        _stateStore.InitializeTimeContext(currentDay, currentTime);
        _eventDispatcher.DispatchSaveLoaded();
        _stateStore.OnSaveLoaded();
        _sessionActive = true;
    }

    internal void HandleDayStarted(DayKey newDay, int currentTime)
    {
        _logger.Debug(LogEvents.LifecycleDayStarted, "Lifecycle event: Day started");
        _stateStore.InitializeTimeContext(newDay, currentTime);
        _eventDispatcher.DispatchDayStarted();
        _stateStore.OnDayStarted(newDay, currentTime);
        _sessionActive = true;
    }

    internal void HandleReturnedToTitle()
    {
        _logger.Debug(LogEvents.LifecycleReturnedToTitle, "Lifecycle event: Returned to title");
        _sessionActive = false;
        _eventDispatcher.DispatchReturnedToTitle();
        _stateStore.OnReturnToTitle();
    }

    internal void HandleSavingInProgress()
    {
        _logger.Debug(LogEvents.LifecycleSavingInProgress, "Lifecycle event: Saving in progress");
        _eventDispatcher.DispatchSavingInProgress();
    }

    internal void HandleTimeChanged(DayKey currentDay, int currentTime)
    {
        if (!_sessionActive)
        {
            return;
        }

        _eventDispatcher.DispatchTimeChanged(currentDay, currentTime);
        _stateStore.OnTimeChanged(currentDay, currentTime);
    }

    internal void HandleUpdateTicked(bool isDebugMode)
    {
        if (isDebugMode)
        {
            _logger.Debug(LogEvents.LifecycleUpdateTickedGuard, "Forwarding throttled tick lifecycle");
        }

        _eventDispatcher.DispatchUpdateTicked();
    }
}
