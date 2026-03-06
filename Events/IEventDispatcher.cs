namespace JojaAutoTasks.Events;

/// <summary>
/// Defines a contract for dispatching lifecycle events to runtime systems.
/// </summary>
internal interface IEventDispatcher
{
    // Responsibilities:
    // - define event dispatching contract for runtime systems
    // - decouple runtime systems from smapi event args and subscription model

    // Non-responsibilities:
    // - no smapi event subscription ownership
    // - no lifecycle sequencing ownership
    // - no persistence implementation
    // - no task/store mutation
    // - no tests in this step

    void DispatchSaveLoaded();
    void DispatchDayStarted();
    void DispatchReturnedToTitle();
    void DispatchSavingInProgress();
    void DispatchUpdateTicked();
}