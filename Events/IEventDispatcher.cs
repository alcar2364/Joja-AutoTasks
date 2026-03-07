// Purpose: Defines the Phase 1 lifecycle signal dispatch contract used by the coordinator to decouple
// SMAPI hook sequencing from runtime processing concerns.
namespace JojaAutoTasks.Events;

/// <summary>
/// Lifecycle signal dispatch contract.
/// </summary>
internal interface IEventDispatcher
{
    // Dependencies

    // State

    // Constants

    // Constructor

    // Public API

    void DispatchGameLaunched();
    void DispatchSaveLoaded();
    void DispatchDayStarted();
    void DispatchReturnedToTitle();
    void DispatchSavingInProgress();
    void DispatchUpdateTicked();

    // Event Handlers

    // Private Helpers
}