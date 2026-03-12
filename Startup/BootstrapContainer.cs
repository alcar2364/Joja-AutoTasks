using StardewModdingAPI;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.Events;
using JojaAutoTasks.State;

namespace JojaAutoTasks.Startup;

/// <summary>Builds the mod runtime object graph.</summary>
internal static class BootstrapContainer
{
    // -- Public API -- //
    internal static ModRuntime Build(IModHelper helper, IMonitor monitor)
    {
        ModLogger logger = new(monitor);
        logger.Info(LogEvents.StartupEntry, "Joja AutoLogger initialized.");

        ModConfig config = new ConfigLoader(helper).Load();
        StateStore stateStore = new();
        EventDispatcher eventDispatcher = new();
        LifecycleCoordinator lifecycleCoordinator = new(logger, eventDispatcher, stateStore);
        
        logger.Info(
            LogEvents.StartupInitialized,
            "Joja AutoTasks initialized. Your productivity is our priority!");

        ModRuntime runtime = new(
            logger,
            config,
            eventDispatcher,
            lifecycleCoordinator,
            stateStore);

        return runtime;
    }
}
