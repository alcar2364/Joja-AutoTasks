using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


namespace JojaAutoTasks;

    /// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    // Public Methods

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.GameLoop.Saving += this.OnSaving;
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        
    }

    private void OnSaving(object? sender, SavingEventArgs e)
    {
        
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        
    }
}

