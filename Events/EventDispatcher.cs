namespace JojaAutoTasks.Events;

/// <summary>
/// Accept lifecycle dispatch calls and record them in call order for deterministic internal observation; perform no downstream processing
/// </summary>
internal sealed class EventDispatcher : IEventDispatcher
{
    // Responsibilities:
    // - concrete implementation of event dispatching contract for runtime systems

    // Non-responsibilities:
    // - no smapi event subscription ownership
    // - no lifecycle sequencing ownership
    // - no persistence implementation
    // - no task/store mutation
    // - no tests in this step

    public void DispatchGameLaunched()
    {
        // No-op for now; will route to runtime systems in later phases.

    }
    
    public void DispatchSaveLoaded()
    {
        // No-op for now; will route to runtime systems in later phases.

    }

    public void DispatchDayStarted()
    {
        // No-op for now; will route to runtime systems in later phases.

    }

    public void DispatchReturnedToTitle()
    {
        // No-op for now; will route to runtime systems in later phases.

    }

    public void DispatchSavingInProgress()
    {
        // No-op for now; will route to runtime systems in later phases.

    }

    public void DispatchUpdateTicked()
    {
        // No-op for now; will route to runtime systems in later phases.

    }
}