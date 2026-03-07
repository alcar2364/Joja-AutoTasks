// Purpose: Reads config.json through SMAPI and returns a normalized ModConfig by defaulting null/invalid
// payloads and reconciling version drift to the current schema version.
using System.Linq.Expressions;
using StardewModdingAPI;

namespace JojaAutoTasks.Configuration;

/// <summary>
/// Loads and normalizes mod config payloads.
/// </summary>
internal sealed class ConfigLoader
{
    // Dependencies
    private readonly IModHelper helper;

    // Constructor
    public ConfigLoader(IModHelper helper)
    {
        this.helper = helper;
    }

    // Public API

    // Load the config, returning a valid config object with defaults applied and any necessary reconciliation performed.
    public ModConfig Load()
    {
        ModConfig? loadedConfig;

        try
        {
            loadedConfig = helper.ReadConfig<ModConfig>();
        }
        catch
        {
            loadedConfig = null;
        }

        return ValidateConfig(loadedConfig);
    }

    // Private Helpers

    // Attempt to reconcile any issues with the loaded config, and return a valid config object
    private ModConfig ValidateConfig(ModConfig? loadedConfig)
    {
        // If the config is missing or invalid, return a default config
        if (loadedConfig is null)
        {
            return new ModConfig();
        }

        // If the config version is older than the current version, attempt to reconcile any differences.
        if (loadedConfig.ConfigVersion < ModConfig.CurrentConfigVersion)
        {
            // Attempts to reconcile differences between config versions, such as missing fields or changed structures.
            // if saved config is older than mod's current config

            return UpgradeFromOlderVersion(loadedConfig); // Update the version after normalization
        }

        if (loadedConfig.ConfigVersion > ModConfig.CurrentConfigVersion)
        {
            //// Attempts to reconcile differences between config versions, such as missing fields or changed structures.
            // if saved config version is newer than mod's current version
            return NormalizeFutureVersion(loadedConfig);
        }

        // If the config is valid and up to date, return it as is.
        return NormalizeCurrentVersion(loadedConfig);
    }

    // Reconcile an older config version to the current version, applying any necessary transformations or defaults.
    // Currently, there are no differences between versions. This is a placeholder for future config changes.
    // TODO: potentially make static
    private ModConfig UpgradeFromOlderVersion(ModConfig oldConfig)
    {
        return CreateNormalizedConfig(oldConfig);
    }

    // Normalizes a future config to the current version, applying defaults and ignoring unknown fields.
    // This is a safe fallback that prevents crashes due to unknown config structures, but does not preserve any settings from the future config.
    // This is an edge case and should rarely happen.
    // Currently, there are no differences between versions. This is a placeholder.
    // TODO: potentially make static
    private ModConfig NormalizeFutureVersion(ModConfig futureConfig)
    {
        return CreateNormalizedConfig(futureConfig);
    }

    // Normalizes a current config, applying defaults for any missing fields. This is a safeguard against 
    // invalid configs that are missing fields, but have the correct version.
    // TODO: potentially make static
    private ModConfig NormalizeCurrentVersion(ModConfig currentConfig)
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