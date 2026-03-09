namespace JojaAutoTasks.Infrastructure.Logging;

/// <summary>Defines stable log event names for the mod runtime.</summary>
internal static class LogEvents
{
    // -- Constants -- //
    public const string StartupEntry = "startup.entry";
    public const string StartupInitialized = "startup.initialized";
    public const string StartupFailed = "startup.failed";

    public const string ConfigLoadStarted = "config.load.started";
    public const string ConfigLoadSucceeded = "config.load.succeeded";
    public const string ConfigLoadDefaulted = "config.load.defaulted";
    public const string ConfigLoadInvalid = "config.load.invalid";
    public const string ConfigVersionMismatch = "config.version.mismatch";

    public const string LifecycleGameLaunched = "lifecycle.game_launched";
    public const string LifecycleSaveLoaded = "lifecycle.save_loaded";
    public const string LifecycleDayStarted = "lifecycle.day_started";
    public const string LifecycleReturnedToTitle = "lifecycle.returned_to_title";
    public const string LifecycleSavingInProgress = "lifecycle.saving.in_progress";
    public const string LifecycleUpdateTickedGuard = "lifecycle.update_ticked.guard";
}
