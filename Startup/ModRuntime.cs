using JojaAutoTasks.Configuration;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;

namespace JojaAutoTasks.Startup;

/// <summary>Aggregates the runtime services composed during startup.</summary>
internal sealed class ModRuntime
{
    // -- Public API -- //
    internal ModLogger Logger { get; }

    internal ModConfig Config { get; }

    internal IEventDispatcher EventDispatcher { get; }

    internal LifecycleCoordinator LifecycleCoordinator { get; }

    // -- Constructor -- //
    internal ModRuntime(
        ModLogger logger,
        ModConfig config,
        IEventDispatcher eventDispatcher,
        LifecycleCoordinator lifecycleCoordinator)
    {
        Logger = logger;
        Config = config;
        EventDispatcher = eventDispatcher;
        LifecycleCoordinator = lifecycleCoordinator;
    }
}
