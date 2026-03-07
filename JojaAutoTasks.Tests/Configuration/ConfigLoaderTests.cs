using JojaAutoTasks.Configuration;
using Moq;
using StardewModdingAPI;
using Xunit;

namespace JojaAutoTasks.Tests.Configuration;

public class ConfigLoaderTests
{
    [Fact]
    public void Load_WhenReadConfigReturnsCurrentVersion_NormalizesAndPreservesFlags()
    {
        ModConfig input = CreateValidConfig();
        Mock<IModHelper> helper = CreateHelperReturning(input);
        ConfigLoader sut = new ConfigLoader(helper.Object);

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
        ConfigLoader sut = new ConfigLoader(helper.Object);

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
        ConfigLoader sut = new ConfigLoader(helper.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigThrows_ReturnsDefaultConfig()
    {
        Mock<IModHelper> helper = new Mock<IModHelper>(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Throws(new InvalidOperationException("Invalid config payload."));
        ConfigLoader sut = new ConfigLoader(helper.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.True(result.EnableMod);
        Assert.False(result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigReturnsNull_ReturnsDefaultConfig()
    {
        Mock<IModHelper> helper = new Mock<IModHelper>(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Returns((ModConfig)null!);
        ConfigLoader sut = new ConfigLoader(helper.Object);

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
        ConfigLoader sut = new ConfigLoader(helper.Object);

        ModConfig result = sut.Load();

        Assert.NotSame(input, result);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

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
        Mock<IModHelper> helper = new Mock<IModHelper>(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Returns(config);
        return helper;
    }
}