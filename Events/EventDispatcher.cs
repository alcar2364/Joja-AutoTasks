// Purpose: Implements the lifecycle dispatch contract as a deterministic no-op in Phase 1, preserving
// explicit boundaries between lifecycle coordination and downstream runtime processing.
namespace JojaAutoTasks.Events;

/// <summary>
/// Deterministic no-op lifecycle dispatcher.
/// </summary>
internal sealed class EventDispatcher : IEventDispatcher
{
    // Dependencies

    // State

    // Constants

    // Constructor

    // Public API

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

    // Event Handlers

    // Private Helpers
}