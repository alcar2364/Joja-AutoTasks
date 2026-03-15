using Moq;
using StardewModdingAPI;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Infrastructure.Logging;


namespace JojaAutoTasks.Tests.Configuration;

/// <summary>Tests configuration normalization and fallback behavior in <see cref="ConfigLoader" />.</summary>
public class ConfigLoaderTests
{
    // -- Public API -- //
    [Fact]
    public void Load_WhenReadConfigReturnsCurrentVersion_NormalizesAndPreservesFlags()
    {
        ModConfig input = CreateValidConfig();
        Mock<IModHelper> helper = CreateHelperReturning(input);
        ModLogger logger = CreateLogger();
        ConfigLoader sut = new ConfigLoader(helper.Object, logger);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Load_WhenReadConfigReturnsOlderVersion_NormalizesAndPreservesFlags(int olderVersion)
    {
        ModConfig input = CreateValidConfig(config =>
        {
            config.ConfigVersion = olderVersion;
            config.EnableMod = false;
            config.EnableDebugMode = true;
        });

        Mock<IModHelper> helper = CreateHelperReturning(input);
        ModLogger logger = CreateLogger();
        ConfigLoader sut = new ConfigLoader(helper.Object, logger);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(int.MaxValue)]
    public void Load_WhenReadConfigReturnsFutureVersion_NormalizesAndPreservesFlags(int futureVersion)
    {
        ModConfig input = CreateValidConfig(config =>
        {
            config.ConfigVersion = futureVersion;
            config.EnableMod = false;
            config.EnableDebugMode = true;
        });

        Mock<IModHelper> helper = CreateHelperReturning(input);
        ModLogger logger = CreateLogger();
        ConfigLoader sut = new ConfigLoader(helper.Object, logger);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigThrows_LogsFallbackAndReturnsDefaultConfig()
    {
        Mock<IModHelper> helper = new(MockBehavior.Strict);
        Mock<IMonitor> monitor = new(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Throws(new InvalidOperationException("Invalid config payload."));
        helper.SetupGet(x => x.DirectoryPath).Returns("C:\\TestMod");
        monitor.Setup(x => x.Log(
            It.Is<string>(message =>
                message.Contains("[config.load.defaulted]")
                && message.Contains("[C:\\TestMod\\config.json]")
                && message.Contains("System.InvalidOperationException")
                && message.Contains("Invalid config payload.")
                && message.Contains("Falling back to default config.")),
            LogLevel.Error));

        ModLogger logger = new(monitor.Object);
        ConfigLoader sut = new(helper.Object, logger);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.True(result.EnableMod);
        Assert.False(result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
        helper.VerifyGet(x => x.DirectoryPath, Times.Once);
        monitor.VerifyAll();
    }

    [Fact]
    public void Load_WhenReadConfigReturnsNull_ReturnsDefaultConfig()
    {
        Mock<IModHelper> helper = new(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Returns((ModConfig)null!);
        ModLogger logger = CreateLogger();
        ConfigLoader sut = new(helper.Object, logger);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.True(result.EnableMod);
        Assert.False(result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigReturnsConfig_AlwaysReturnsNewInstance()
    {
        ModConfig input = CreateValidConfig(config =>
        {
            config.EnableMod = false;
            config.EnableDebugMode = true;
        });

        Mock<IModHelper> helper = CreateHelperReturning(input);
        ModLogger logger = CreateLogger();
        ConfigLoader sut = new(helper.Object, logger);

        ModConfig result = sut.Load();

        Assert.NotSame(input, result);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    // -- Private Helpers -- //
    private static ModConfig CreateValidConfig(Action<ModConfig>? configure = null)
    {
        ModConfig config = new ModConfig
        {
            ConfigVersion = ModConfig.CurrentConfigVersion,
            EnableMod = true,
            EnableDebugMode = false
        };

        configure?.Invoke(config);
        return config;
    }

    private static Mock<IModHelper> CreateHelperReturning(ModConfig config)
    {
        Mock<IModHelper> helper = new(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Returns(config);
        return helper;
    }

    private static ModLogger CreateLogger()
    {
        Mock<IMonitor> monitor = new(MockBehavior.Loose);
        return new ModLogger(monitor.Object);
    }
}
