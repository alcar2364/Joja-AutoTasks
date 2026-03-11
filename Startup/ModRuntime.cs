using JojaAutoTasks.Configuration;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.State;

namespace JojaAutoTasks.Startup;

/// <summary>Aggregates the runtime services composed during startup.</summary>
internal sealed class ModRuntime
{
    // -- Public API -- //
    internal ModLogger Logger { get; }

    internal ModConfig Config { get; }

    internal IEventDispatcher EventDispatcher { get; }

    internal LifecycleCoordinator LifecycleCoordinator { get; }

    internal StateStore StateStore { get; }

    // -- Constructor -- //
    internal ModRuntime(
        ModLogger logger,
        ModConfig config,
        IEventDispatcher eventDispatcher,
        LifecycleCoordinator lifecycleCoordinator,
        StateStore stateStore)
    {
        Logger = logger;
        Config = config;
        EventDispatcher = eventDispatcher;
        LifecycleCoordinator = lifecycleCoordinator;
        StateStore = stateStore;
    }
}
