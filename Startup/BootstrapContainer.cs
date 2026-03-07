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
        var logger = new ModLogger(monitor);
        logger.Info(LogEvents.StartupEntry, "Joja AutoLogger initialized.");

        // Load configuration
        var config = new ConfigLoader(helper).Load();

        // Instantiate event dispatcher
        var eventDispatcher = new EventDispatcher();

        // Instantiate lifecycle coordinator
        var lifecycleCoordinator = new LifecycleCoordinator(logger, eventDispatcher);

        logger.Info(
        LogEvents.StartupInitialized, "Joja AutoTasks initialized. Your productivity is our priority!"
        );

        var runtime = new ModRuntime
        (
            logger,
            config,
            eventDispatcher,
            lifecycleCoordinator
        );

        return runtime;
    }

}