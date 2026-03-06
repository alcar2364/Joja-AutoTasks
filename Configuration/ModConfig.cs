namespace JojaAutoTasks.Configuration;

/// <summary>
/// The mod configuration, which is loaded from a JSON file in the mod directory. 
/// ConfigVersion is used to manage config changes across versions.
/// It should be incremented whenever the config structure changes in a way that would invalidate old configs.
/// </summary>
internal sealed class ModConfig
{
   public const int CurrentConfigVersion = 1;
   public int ConfigVersion { get; set; } = CurrentConfigVersion;
   public bool EnableMod { get; set; } = true;
   public bool EnableDebugMode { get; set; } = false;
}