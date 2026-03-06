using System;
using Microsoft.Xna.Framework;
using JojaAutoTasks.Infrastructure.Logging;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace JojaAutoTasks;

    /// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    // Dependencies

    private ModLogger? logger;
    private uint nextTickLogAt;

    // Public Methods

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // Confirm startup
        logger = new ModLogger(Monitor);
        logger.Info(LogEvents.StartupEntry, "Joja AutoTasks initialized. Your productivity is our priority!");

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.GameLoop.Saving += this.OnSaving;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

        logger.Info(LogEvents.StartupInitialized, "We have hooked into your life. Let's make every day a Joja day!");

    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        logger?.Debug(LogEvents.LifecycleSaveLoaded, "Lifecycle event: Save loaded");
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        logger?.Debug(LogEvents.LifecycleDayStarted, "Lifecycle event: Day started");
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        logger?.Debug(LogEvents.LifecycleReturnedToTitle, "Lifecycle event: Returned to title");
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        logger?.Debug(LogEvents.LifecycleSavingSignal, "Lifecycle event: Saving");
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (e.Ticks < nextTickLogAt)
        {
            return;
        }

        nextTickLogAt = e.Ticks + 360; // Log every 6 seconds (60 ticks per second)
        logger?.Trace(LogEvents.LifecycleUpdateTickedGuard, "Tick Logging with Throttle guard activated");
    }
}

