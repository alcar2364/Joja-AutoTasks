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
        // Listen for the DayStarted event, which happens after the player wakes up and the world is ready.
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Ignore if player hasn't loaded a save yet.
        if (!Context.IsWorldReady)
            return;

        // Print button presses to the console
        this.Monitor.Log($"The {e.Button} button was pressed.", LogLevel.Debug);
    }
}

