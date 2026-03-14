using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Events;

/// <summary>Defines lifecycle signals emitted by the mod runtime.</summary>
internal interface IEventDispatcher
{
    // -- Public API -- //
    void DispatchGameLaunched();

    void DispatchSaveLoaded();

    void DispatchDayStarted();

    void DispatchReturnedToTitle();

    void DispatchSavingInProgress();

    void DispatchTimeChanged(DayKey currentDay, int currentTime);

    void DispatchUpdateTicked();
}
