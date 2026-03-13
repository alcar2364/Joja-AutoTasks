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

        // BootstrapContainer constructs StateStore before ModRuntime is returned, so
        // subscription setup is safe once the runtime-owned store reference is
        // available. SubscriptionManager initialization and subscription depends
        // on successful runtime composition.

        UiSnapshotSubscriptionManager.Initialize(_runtime.StateStore);

        Monitor.Log("Subscribing to task snapshot updates.", LogLevel.Trace);
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
        // Dispose of the snapshot before saving to prevent memory leaks
        // Guarded so that if saving occurs before subscription is established, 
        // it won't throw null-reference
        _runtime.LifecycleCoordinator.HandleSavingInProgress();
        if (_snapshotSubscriptionToken is not null)
        {   
            Monitor.Log("Disposing of task snapshot subscription before saving.", LogLevel.Trace);
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
