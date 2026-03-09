namespace JojaAutoTasks.Configuration;

/// <summary>Represents the persisted mod configuration schema.</summary>
internal sealed class ModConfig
{
    // -- Constants -- //
    public const int CurrentConfigVersion = 1;

    // -- Public API -- //
    public int ConfigVersion { get; set; } = CurrentConfigVersion;

    public bool EnableMod { get; set; } = true;

    public bool EnableDebugMode { get; set; } = false;
}
