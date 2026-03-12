using StardewModdingAPI;
using StardewModdingAPI.Events;
using JojaAutoTasks.Startup;
using JojaAutoTasks.Ui;

namespace JojaAutoTasks;

/// <summary>Forwards SMAPI lifecycle hooks into the mod runtime.</summary>
internal sealed class ModEntry : Mod
{
    // -- Dependencies -- //
    private ModRuntime _runtime = null!;

    // -- State -- //
    private uint _nextTickLogAt;
    private IDisposable? _snapshotSubscriptionToken;

    // -- Public API -- //
    /// <summary>Initializes the mod entry point.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        _runtime = BootstrapContainer.Build(helper, Monitor);

        // Initialize UI snapshot subscription manager. Cannot be null after this point
        // because StateStore initialization is guaranteed to have completed.
        UiSnapshotSubscriptionManager.Initialize(_runtime.StateStore);
        _snapshotSubscriptionToken = UiSnapshotSubscriptionManager.Subscribe(_snapshot => { });

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
        // Dispose of the snapshot after saving to prevent memory leaks
        // Guarded so that if saving occurs before subscription is established, 
        // it won't throw null-reference
        _runtime.LifecycleCoordinator.HandleSavingInProgress();
        if (_snapshotSubscriptionToken is not null)
        {
            _snapshotSubscriptionToken.Dispose();
            _snapshotSubscriptionToken = null;
        }
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!ShouldForwardUpdateTick(e.Ticks))
        {
            return;
        }

        _runtime.LifecycleCoordinator.HandleUpdateTicked(_runtime.Config.EnableDebugMode);
    }

    // -- Private Helpers -- //
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
