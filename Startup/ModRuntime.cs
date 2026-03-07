using JojaAutoTasks.Configuration;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;

namespace JojaAutoTasks.Startup;
/// <summary> The mod runtime, which holds references to all major dependencies and provides a 
/// single access point for core services. </summary>

internal sealed class ModRuntime
{
    internal ModLogger Logger { get; }

    internal ModConfig Config { get; }

    internal IEventDispatcher EventDispatcher { get; }

    internal LifecycleCoordinator LifecycleCoordinator { get; }
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
