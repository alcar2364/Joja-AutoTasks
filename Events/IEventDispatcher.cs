namespace JojaAutoTasks.Events;

/// <summary>
/// Defines a contract for dispatching lifecycle events to runtime systems.
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

    internal void DispatchSaveLoaded();
    internal void DispatchDayStarted();
    internal void DispatchReturnedToTitle();
    internal void DispatchSaving();
    internal void DispatchUpdateTicked();
}