using System;
using Microsoft.Xna.Framework;
using JojaAutoTasks.Infrastructure.Logging;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Lifecycle;


namespace JojaAutoTasks;

    /// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    // Dependencies

    // Potentially change to closure-based approach
    private ModLogger logger = null!;
    private uint nextTickLogAt;

    // TODO: Loaded during startup; used by runtime systems in later phases. Can delete this comment once it is referenced in the code.
    private ModConfig config = new();

    //TODO: potentially change to closure-based approach
    private LifecycleCoordinator lifecycleCoordinator = null!;

    // Public Methods

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper )
    {
        // Confirm startup
        logger = new ModLogger(Monitor);
        logger.Info(LogEvents.StartupEntry, "Joja AutoTasks initialized. Your productivity is our priority!");

        // Load configuration
        // TODO: add debug log message around config loading
        ConfigLoader configLoader = new ConfigLoader(helper);
        config = configLoader.Load();

        //Instantiate lifecycle coordinator
        lifecycleCoordinator = new LifecycleCoordinator(logger);

        // Lifecycle hooks
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.Saving += OnSaving;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

        logger.Info(LogEvents.StartupInitialized, "We have hooked into your life. Let's make every day a Joja day!");

    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        lifecycleCoordinator.HandleGameLaunched();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        lifecycleCoordinator.HandleSaveLoaded();
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        logger.Debug(LogEvents.LifecycleDayStarted, "Lifecycle event: Day started");
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        logger.Debug(LogEvents.LifecycleReturnedToTitle, "Lifecycle event: Returned to title");
    }

    /// <summary>
    /// OnSaving is signal-only. Do not perform file writes, checkpoint creation, or persistence work from this path.
    /// </summary>
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        lifecycleCoordinator.HandleSavingInProgress();
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (e.Ticks < nextTickLogAt)
        {
            return;
        }

        nextTickLogAt = e.Ticks + 360; // Log every 6 seconds (60 ticks per second)
        logger.Trace(LogEvents.LifecycleUpdateTickedGuard, "Tick Logging with Throttle guard activated");
    }
}

