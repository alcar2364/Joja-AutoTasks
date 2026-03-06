using System.Collections.Concurrent;

internal sealed class ModConfig
{
   public const int CurrentConfigVersion = 1;
   public int ConfigVersion { get; set; } = CurrentConfigVersion;
   public bool EnableMod { get; set; } = true;
   public bool EnableDebugMode { get; set; } = false;
}