namespace JojaAutoTasks.Events;

/// <summary>Implements lifecycle dispatch as a deterministic Phase 1 no-op.</summary>
internal sealed class EventDispatcher : IEventDispatcher
{
    // -- Public API -- //
    public void DispatchGameLaunched()
    {
    }

    public void DispatchSaveLoaded()
    {
    }

    public void DispatchDayStarted()
    {
    }

    public void DispatchReturnedToTitle()
    {
    }

    public void DispatchSavingInProgress()
    {
    }

    public void DispatchUpdateTicked()
    {
    }
}
