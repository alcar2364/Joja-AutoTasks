using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Events;
namespace JojaAutoTasks.Lifecycle;

/// <summary>
/// Coordinates approved lifecycle signal flow for the mod. Owns sequencing and boundaries only. 
/// Does not execute persistence, dispatcher processing, or gameplay mutation.
/// </summary>  
internal sealed class LifecycleCoordinator
{
    // Responsibilities:
    // - Define approved entry points for lifecycle signals
    // - preserve deterministic sequencing and boundaries of lifecycle signals
    // - keeps lifecycle sequencing in one place

    // Non-responsibilities:
    // - no smapi event subscription ownership
    // - no dispatch implementation
    // - no persistence implementation
    // - no task/store mutation
    // - no tests in this step

    // -- Dependencies -- //

    private readonly ModLogger logger;
    private readonly IEventDispatcher eventDispatcher;

    // -- Constructor -- //
    internal LifecycleCoordinator(ModLogger logger, IEventDispatcher eventDispatcher)
    {
        this.logger = logger;
        this.eventDispatcher = eventDispatcher;
    }
    
    // -- Lifecycle Signal Handlers -- //

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
    
    internal void HandleUpdateTicked()
    {
        logger.Debug(LogEvents.LifecycleUpdateTickedGuard, "Forwarding throttled tick lifecycle");
        eventDispatcher.DispatchUpdateTicked();
    }

}