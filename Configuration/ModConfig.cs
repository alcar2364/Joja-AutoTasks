// Purpose: Defines the persisted mod configuration schema used by config.json, including versioning
// metadata and Phase 1 runtime flags consumed during startup and lifecycle routing.
namespace JojaAutoTasks.Configuration;

/// <summary>
/// Persisted Joja AutoTasks configuration model.
/// </summary>
internal sealed class ModConfig
{
   // Constants
   public const int CurrentConfigVersion = 1;

   // Public API
   public int ConfigVersion { get; set; } = CurrentConfigVersion;
   public bool EnableMod { get; set; } = true;
   public bool EnableDebugMode { get; set; } = false;
}