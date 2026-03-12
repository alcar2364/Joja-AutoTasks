using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Events;
using JojaAutoTasks.State;

namespace JojaAutoTasks.Lifecycle;

/// <summary>Coordinates lifecycle logging and dispatcher forwarding.</summary>
internal sealed class LifecycleCoordinator
{
    // -- Dependencies -- //
    private readonly ModLogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly StateStore _stateStore;

    // -- Constructor -- //
    internal LifecycleCoordinator(ModLogger logger, IEventDispatcher eventDispatcher, StateStore stateStore)
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _stateStore = stateStore;
    }

    // -- Public API -- //
    internal void HandleGameLaunched()
    {
        _logger.Debug(LogEvents.LifecycleGameLaunched, "Lifecycle event: Game launched");
        _eventDispatcher.DispatchGameLaunched();
    }

    internal void HandleSaveLoaded()
    {
        _logger.Debug(LogEvents.LifecycleSaveLoaded, "Lifecycle event: Save loaded");
        _eventDispatcher.DispatchSaveLoaded();
        _stateStore.OnSaveLoaded();
    }

    internal void HandleDayStarted()
    {
        _logger.Debug(LogEvents.LifecycleDayStarted, "Lifecycle event: Day started");
        _eventDispatcher.DispatchDayStarted();
    }

    internal void HandleReturnedToTitle()
    {
        _logger.Debug(LogEvents.LifecycleReturnedToTitle, "Lifecycle event: Returned to title");
        _eventDispatcher.DispatchReturnedToTitle();
    }

    internal void HandleSavingInProgress()
    {
        _logger.Debug(LogEvents.LifecycleSavingInProgress, "Lifecycle event: Saving in progress");
        _eventDispatcher.DispatchSavingInProgress();
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
