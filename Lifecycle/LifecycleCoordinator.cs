// Purpose: Coordinates approved lifecycle signal sequencing by logging and forwarding each hook event
// to the dispatcher while enforcing Phase 1 signal-only boundaries.
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Events;

namespace JojaAutoTasks.Lifecycle;

/// <summary>
/// Lifecycle signal sequencer and dispatcher forwarder.
/// </summary>  
internal sealed class LifecycleCoordinator
{
    // Dependencies

    private readonly ModLogger logger;
    private readonly IEventDispatcher eventDispatcher;

    // State

    // Constants

    // Constructor
    internal LifecycleCoordinator(ModLogger logger, IEventDispatcher eventDispatcher)
    {
        this.logger = logger;
        this.eventDispatcher = eventDispatcher;
    }

    // Public API

    internal void HandleGameLaunched()
    {
        logger.Debug(LogEvents.LifecycleGameLaunched, "Lifecycle event: Game launched");
        eventDispatcher.DispatchGameLaunched();
    }

    internal void HandleSaveLoaded()
    {
        logger.Debug(LogEvents.LifecycleSaveLoaded, "Lifecycle event: Save loaded");
        eventDispatcher.DispatchSaveLoaded();
    }

    internal void HandleDayStarted()
    {
        logger.Debug(LogEvents.LifecycleDayStarted, "Lifecycle event: Day started");
        eventDispatcher.DispatchDayStarted();
    }

    internal void HandleReturnedToTitle()
    {
        logger.Debug(LogEvents.LifecycleReturnedToTitle, "Lifecycle event: Returned to title");
        eventDispatcher.DispatchReturnedToTitle();
    }

    internal void HandleSavingInProgress()
    {
        logger.Debug(LogEvents.LifecycleSavingInProgress, "Lifecycle event: Saving in progress");
        eventDispatcher.DispatchSavingInProgress();
    }

    internal void HandleUpdateTicked(bool isDebugMode)
    {
        if (isDebugMode)
        {
            logger.Debug(LogEvents.LifecycleUpdateTickedGuard, "Forwarding throttled tick lifecycle");
        }
        eventDispatcher.DispatchUpdateTicked();
    }

    // Event Handlers

    // Private Helpers
}