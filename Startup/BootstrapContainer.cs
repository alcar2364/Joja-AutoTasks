using StardewModdingAPI;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.Events;

namespace JojaAutoTasks.Startup;

/// <summary> Builds the mod runtime and composes all dependencies. </summary>
internal static class BootstrapContainer
{
    internal static ModRuntime Build(IModHelper helper, IMonitor monitor)
    {

        // Initialize logger
        ModLogger logger = new(monitor);
        logger.Info(LogEvents.StartupEntry, "Joja AutoLogger initialized.");

        // Load configuration
        ModConfig config = new ConfigLoader(helper).Load();

        // Instantiate event dispatcher
        EventDispatcher eventDispatcher = new();

        // Instantiate lifecycle coordinator
        LifecycleCoordinator lifecycleCoordinator = new(logger, eventDispatcher);
        logger.Info(
        LogEvents.StartupInitialized, "Joja AutoTasks initialized. Your productivity is our priority!"
        );

        ModRuntime runtime = new(
            logger,
            config,
            eventDispatcher,
            lifecycleCoordinator
        );

        return runtime;
    }

}