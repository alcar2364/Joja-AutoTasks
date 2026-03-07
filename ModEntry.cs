// Purpose: Hosts the SMAPI entrypoint for Joja AutoTasks and forwards subscribed game-loop signals
// into the runtime lifecycle coordinator with a throttled update-tick guard.

using StardewModdingAPI;
using StardewModdingAPI.Events;
using JojaAutoTasks.Startup;

namespace JojaAutoTasks;

/// <summary>SMAPI mod entrypoint and hook forwarding.</summary>
internal sealed class ModEntry : Mod
{
    // Dependencies

    // ModRuntime is the composition root for the mod. It holds references to all major dependencies
    // and provides a single access point for core services.
    private ModRuntime _runtime = null!; 

    // State
    private uint _nextTickLogAt;

    // Public API

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // Build the runtime, which will initialize all dependencies and perform necessary setup work.
        _runtime = BootstrapContainer.Build(helper, Monitor);


        // Lifecycle hooks
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

    }

    // Event Handlers
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

    /// <summary>
    /// OnSaving is signal-only. Do not perform file writes, checkpoint creation, or persistence work from this path.
    /// </summary>
    private void OnSaving(object? sender, SavingEventArgs e)
    {

        _runtime.LifecycleCoordinator.HandleSavingInProgress();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!ShouldForwardUpdateTick(e.Ticks))
        {
            return;
        }
        _runtime.LifecycleCoordinator.HandleUpdateTicked(_runtime.Config.EnableDebugMode);
    }

    // Private Helpers

    // Helper method to throttle UpdateTicked signals to a reasonable frequency. Adjust as needed
    private bool ShouldForwardUpdateTick(uint currentTick)
    {
        if (currentTick >= _nextTickLogAt)
        {
            _nextTickLogAt = currentTick + 360; // Throttles ticks to once every 6 seconds (360 ticks) 
            return true;
        }
        return false;
    }
}

