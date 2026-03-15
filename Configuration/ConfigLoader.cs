using JojaAutoTasks.Infrastructure.Logging;
using StardewModdingAPI;

namespace JojaAutoTasks.Configuration;

/// <summary>Loads mod configuration and normalizes schema version drift.</summary>
internal sealed class ConfigLoader
{
    // -- Dependencies -- //
    private readonly IModHelper _helper;
    private readonly ModLogger _logger;

    // -- Constructor -- //
    internal ConfigLoader(IModHelper helper, ModLogger logger)
    {
        _helper = helper;
        _logger = logger;
    }

    // -- Public API -- //

    /// <summary>
    ///  Loads the mod configuration from file, applying normalization and fallback as needed.
    /// </summary>
    // This class is only doing config read, normalization, fallback, and logging, not introducing new schema or migration rules.
    // Fallback order:
    // 1. read config
    // 2. if read fails or is null, use defaults
    // 3. if version is older, normalize using existing older version path
    // 4. if version is newer, normalize using existing future version path
    // 5. if version is current, normalize current version

    internal ModConfig Load()
    {
        ModConfig? loadedConfig;

        try
        {
            loadedConfig = _helper.ReadConfig<ModConfig>();
        }
        // Exception thrown during ReadConfig logs the fallback reason
        catch (Exception ex)
            when (ex is not (StackOverflowException or OutOfMemoryException or ThreadAbortException)
            )
        {
            string configPath = Path.Combine(_helper.DirectoryPath, "config.json");

            // log one structured error message that includes: config path attempted, exception type, exception message,inner-exception chain summary, fallback action taken
            _logger.Error(
                LogEvents.ConfigLoadDefaulted,
                configPath,
                $"{ex.GetType().FullName}: {ex.Message}, {GetInnerExceptionChainSummary(ex)}, Falling back to default config."
            );
            loadedConfig = null;
        }

        return ValidateConfig(loadedConfig);
    }

    // -- Private Helpers -- //
    private static ModConfig ValidateConfig(ModConfig? loadedConfig)
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
            EnableDebugMode = sourceConfig.EnableDebugMode,
        };
    }

    private static string GetInnerExceptionChainSummary(Exception ex)
    {
        List<string> innerExceptions = new();

        for (
            Exception? innerEx = ex.InnerException;
            innerEx is not null;
            innerEx = innerEx.InnerException
        )
        {
            string typeName = innerEx.GetType().FullName ?? innerEx.GetType().Name;
            innerExceptions.Add($"{typeName}: {innerEx.Message}");
        }

        return innerExceptions.Count > 0
            ? string.Join(" -> ", innerExceptions)
            : "No inner exceptions.";
    }
}
