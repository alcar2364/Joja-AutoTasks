using StardewModdingAPI;
using StardewModdingAPI.Events;
using JojaAutoTasks.Startup;

namespace JojaAutoTasks;

/// <summary>Forwards SMAPI lifecycle hooks into the mod runtime.</summary>
internal sealed class ModEntry : Mod
{
    // -- Dependencies -- //
    private ModRuntime _runtime = null!;

    // -- State -- //
    private uint _nextTickLogAt;

    // -- Public API -- //
    /// <summary>Initializes the mod entry point.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        _runtime = BootstrapContainer.Build(helper, Monitor);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    // -- Event Handlers -- //
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleGameLaunched();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleSaveLoaded();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleDayStarted();
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleReturnedToTitle();
    }

    // Saving remains signal-only so persistence work never runs inside SMAPI's saving hook.
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleSavingInProgress();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        ForwardUpdateTickIfDue(e.Ticks);
    }

    // -- Private Helpers -- //
    internal void ForwardUpdateTickIfDue(uint ticks)
    {
        if (!ShouldForwardUpdateTick(ticks))
        {
            return;
        }

        _runtime.LifecycleCoordinator.HandleUpdateTicked(_runtime.Config.EnableDebugMode);
    }

    private bool ShouldForwardUpdateTick(uint currentTick)
    {
        if (currentTick >= _nextTickLogAt)
        {
            // SMAPI ticks run at 60 Hz, so 360 ticks keeps forwarding at roughly six-second intervals.
            _nextTickLogAt = currentTick + 360;
            return true;
        }

        return false;
    }
}
