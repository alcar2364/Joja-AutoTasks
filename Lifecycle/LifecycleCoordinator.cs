using System.Diagnostics.Tracing;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using JojaAutoTasks.Infrastructure.Logging;
namespace JojaAutoTasks.Lifecycle;

/// <summary>
/// Coordinates approved lifecycle signal flow for the mod. Owns sequencing and boundaries only. 
/// Does not execute persistence, dispatcher processing, or gameplay mutation.
/// </summary>  
internal sealed class LifecycleCoordinator
{
    // Responsibilities:
    // - Define approved entry points for lifecycle signals
    // - preserve deterministic sequencing and boundaries of lifecycle signals
    // - keeps lifecycle sequencing in one place

    // Non-responsibilities:
    // - no smapi event subscription ownership
    // - no dispatch implementation
    // - no persistence implementation
    // - no task/store mutation
    // - no tests in this step

    // Dependencies:
    private readonly ModLogger logger;

    // Constructor
    internal LifecycleCoordinator(ModLogger logger)
    {
        this.logger = logger;
    }
    
    // TODO: route lifecycle signals with methods on this class, and have the smapi event handlers call those methods.

    internal void HandleGameLaunched()
    {
        logger.Debug(LogEvents.LifecycleGameLaunched, "Lifecycle event: Game launched");
    }
    
    internal void HandleSaveLoaded()
    {
        logger.Debug(LogEvents.LifecycleSaveLoaded, "Lifecycle event: Save loaded");
    }

    internal void HandleSavingInProgress()
    {
        logger.Debug(LogEvents.LifecycleSavingSignal, "Lifecycle event: Saving in progress");
    }
    
}