using StardewModdingAPI;

namespace JojaAutoTasks.Configuration;

/// <summary>Loads mod configuration and normalizes schema version drift.</summary>
internal sealed class ConfigLoader
{
    // -- Dependencies -- //
    private readonly IModHelper _helper;

    // -- Constructor -- //
    internal ConfigLoader(IModHelper helper)
    {
        _helper = helper;
    }

    // -- Public API -- //
    internal ModConfig Load()
    {
        ModConfig? loadedConfig;

        try
        {
            loadedConfig = _helper.ReadConfig<ModConfig>();
        }
        catch
        {
            loadedConfig = null;
        }

        return ValidateConfig(loadedConfig);
    }

    // -- Private Helpers -- //
    private ModConfig ValidateConfig(ModConfig? loadedConfig)
    {
        if (loadedConfig is null)
        {
            return new ModConfig();
        }

        if (loadedConfig.ConfigVersion < ModConfig.CurrentConfigVersion)
        {
            return UpgradeFromOlderVersion(loadedConfig);
        }

        if (loadedConfig.ConfigVersion > ModConfig.CurrentConfigVersion)
        {
            return NormalizeFutureVersion(loadedConfig);
        }

        return NormalizeCurrentVersion(loadedConfig);
    }

    private static ModConfig UpgradeFromOlderVersion(ModConfig oldConfig)
    {
        return CreateNormalizedConfig(oldConfig);
    }

    private static ModConfig NormalizeFutureVersion(ModConfig futureConfig)
    {
        // Future config payloads are reduced to the current schema so newer unknown fields cannot block startup.
        return CreateNormalizedConfig(futureConfig);
    }

    private static ModConfig NormalizeCurrentVersion(ModConfig currentConfig)
    {
        return CreateNormalizedConfig(currentConfig);
    }

    private static ModConfig CreateNormalizedConfig(ModConfig sourceConfig)
    {
        return new ModConfig
        {
            ConfigVersion = ModConfig.CurrentConfigVersion,
            EnableMod = sourceConfig.EnableMod,
            EnableDebugMode = sourceConfig.EnableDebugMode
        };
    }
}
