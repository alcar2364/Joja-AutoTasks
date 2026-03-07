// Purpose: Centralizes stable log event identifiers used by startup, configuration, and lifecycle
// logging so event naming remains deterministic across call sites.

namespace JojaAutoTasks.Infrastructure.Logging;

/// <summary>Stable log event identifier catalog.</summary>
internal static class LogEvents
{
    // Dependencies

    // State

    // Constants

    // Startup constants
    public const string StartupEntry = "startup.entry";
    public const string StartupInitialized = "startup.initialized";
    public const string StartupFailed = "startup.failed";

    // Config constants
    public const string ConfigLoadStarted = "config.load.started";
    public const string ConfigLoadSucceeded = "config.load.succeeded";
    public const string ConfigLoadDefaulted = "config.load.defaulted";
    public const string ConfigLoadInvalid = "config.load.invalid";
    public const string ConfigVersionMismatch = "config.version.mismatch";

    // Lifecycle constants
    public const string LifecycleGameLaunched = "lifecycle.game_launched";
    public const string LifecycleSaveLoaded = "lifecycle.save_loaded";
    public const string LifecycleDayStarted = "lifecycle.day_started";
    public const string LifecycleReturnedToTitle = "lifecycle.returned_to_title";
    public const string LifecycleSavingInProgress = "lifecycle.saving.in_progress";
    public const string LifecycleUpdateTickedGuard = "lifecycle.update_ticked.guard";

    // Constructor

    // Public API

    // Event Handlers

    // Private Helpers
}