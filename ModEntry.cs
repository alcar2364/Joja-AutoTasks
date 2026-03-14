using JojaAutoTasks.Domain.Identifiers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using JojaAutoTasks.Startup;
using JojaAutoTasks.Ui;

namespace JojaAutoTasks;

/// <summary>Forwards SMAPI lifecycle hooks into the mod runtime.</summary>
internal sealed class ModEntry : Mod
{
    // -- Dependencies -- //
    private ModRuntime _runtime = null!;
    private readonly ModEntryHudLifecycle _hudLifecycle = new();

    // -- State -- //
    private uint _nextTickLogAt;

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
        UiToastSubscriptionManager.Initialize(_runtime.StateStore);

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
    }

    // -- Event Handlers -- //
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleGameLaunched();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        DayKey currentDay = CreateCurrentDayKey();
        int currentTime = Game1.timeOfDay;
        _runtime.LifecycleCoordinator.HandleSaveLoaded(currentDay, currentTime);
        _hudLifecycle.HandleSaveLoaded();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        DayKey currentDay = CreateCurrentDayKey();
        int currentTime = Game1.timeOfDay;
        _hudLifecycle.HandleDayStarted();
        _runtime.LifecycleCoordinator.HandleDayStarted(currentDay, currentTime);
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleReturnedToTitle();
        _hudLifecycle.HandleReturnedToTitle();
    }

    // Saving remains signal-only so persistence work never runs inside SMAPI's saving hook.
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        _runtime.LifecycleCoordinator.HandleSavingInProgress();
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        DayKey currentDay = CreateCurrentDayKey();
        int currentTime = Game1.timeOfDay;
        _runtime.LifecycleCoordinator.HandleTimeChanged(currentDay, currentTime);
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

    private static DayKey CreateCurrentDayKey()
    {
        return DayKeyFactory.Create(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
    }
}

// TODO: Should not be another lifecycle coordinator class. Refactor to unify with ModRuntime lifecycle coordination or merge into ModRuntime directly.
internal sealed class ModEntryHudLifecycle
{
    private readonly Func<HudViewModel> _createViewModel;
    private readonly Func<HudViewModel, HudHost> _createHost;
    private HudHost? _hudHost;
    private HudViewModel? _hudViewModel;

    internal ModEntryHudLifecycle()
        : this(
            createViewModel: static () => new HudViewModel(),
            createHost: static viewModel => new HudHost(viewModel))
    {
    }

    internal ModEntryHudLifecycle(
        Func<HudViewModel> createViewModel,
        Func<HudViewModel, HudHost> createHost)
    {
        _createViewModel = createViewModel;
        _createHost = createHost;
    }

    internal HudViewModel? CurrentViewModel => _hudViewModel;

    internal void HandleSaveLoaded()
    {
        _hudViewModel = _createViewModel();
        _hudHost = _createHost(_hudViewModel);
    }

    internal void HandleDayStarted()
    {
        DisposeCurrent();
        _hudViewModel = _createViewModel();
        _hudHost = _createHost(_hudViewModel);
    }

    internal void HandleReturnedToTitle()
    {
        DisposeCurrent();
        _hudHost = null;
        _hudViewModel = null;
    }

    private void DisposeCurrent()
    {
        _hudHost?.Dispose();
        _hudViewModel?.Dispose();
    }
}
