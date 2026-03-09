using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Events;

namespace JojaAutoTasks.Lifecycle;

/// <summary>Coordinates lifecycle logging and dispatcher forwarding.</summary>
internal sealed class LifecycleCoordinator
{
    // -- Dependencies -- //
    private readonly ModLogger _logger;
    private readonly IEventDispatcher _eventDispatcher;

    // -- Constructor -- //
    internal LifecycleCoordinator(ModLogger logger, IEventDispatcher eventDispatcher)
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
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
