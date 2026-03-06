namespace JojaAutoTasks.Infrastructure.Logging;

/// <summary> Defines event IDs for logging throughout the mod. </summary>
internal static class LogEvents
{
    // Startup
    public const string StartupEntry = "startup.entry";
    public const string StartupInitialized = "startup.initialized";
    public const string StartupFailed = "startup.failed";

    // Config
    public const string ConfigLoadStarted = "config.load.started";
    public const string ConfigLoadSucceeded = "config.load.succeeded";
    public const string ConfigLoadDefaulted = "config.load.defaulted";
    public const string ConfigLoadInvalid = "config.load.invalid";
    public const string ConfigVersionMismatch = "config.version.mismatch";

    // Lifecycle
    public const string LifecycleGameLaunched = "lifecycle.game_launched";
    public const string LifecycleSaveLoaded = "lifecycle.save_loaded";
    public const string LifecycleDayStarted = "lifecycle.day_started";
    public const string LifecycleReturnedToTitle = "lifecycle.returned_to_title";
    public const string LifecycleSavingSignal = "lifecycle.saving.signal";
    public const string LifecycleUpdateTickedGuard = "lifecycle.update_ticked.guard";
}