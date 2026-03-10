using JojaAutoTasks.Configuration;
using Moq;
using Xunit;

namespace JojaAutoTasks.Tests.Configuration;

/// <summary>Tests configuration normalization and fallback behavior in <see cref="ConfigLoader" />.</summary>
public class ConfigLoaderTests
{
    // -- Public API -- //
    [Fact]
    public void Load_WhenReadConfigReturnsCurrentVersion_NormalizesAndPreservesFlags()
    {
        ModConfig input = CreateValidConfig();
        Mock<IConfigReader> reader = CreateReaderReturning(input);
        ConfigLoader sut = new ConfigLoader(reader.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
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

        Mock<IConfigReader> reader = CreateReaderReturning(input);
        ConfigLoader sut = new ConfigLoader(reader.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
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

        Mock<IConfigReader> reader = CreateReaderReturning(input);
        ConfigLoader sut = new ConfigLoader(reader.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.NotSame(input, result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.Equal(input.EnableMod, result.EnableMod);
        Assert.Equal(input.EnableDebugMode, result.EnableDebugMode);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigThrows_ReturnsDefaultConfig()
    {
        Mock<IConfigReader> reader = new Mock<IConfigReader>(MockBehavior.Strict);
        reader.Setup(x => x.ReadConfig<ModConfig>()).Throws(new InvalidOperationException("Invalid config payload."));
        ConfigLoader sut = new ConfigLoader(reader.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.True(result.EnableMod);
        Assert.False(result.EnableDebugMode);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigReturnsNull_ReturnsDefaultConfig()
    {
        Mock<IConfigReader> reader = new Mock<IConfigReader>(MockBehavior.Strict);
        reader.Setup(x => x.ReadConfig<ModConfig>()).Returns((ModConfig)null!);
        ConfigLoader sut = new ConfigLoader(reader.Object);

        ModConfig result = sut.Load();

        Assert.NotNull(result);
        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.True(result.EnableMod);
        Assert.False(result.EnableDebugMode);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenReadConfigReturnsConfig_AlwaysReturnsNewInstance()
    {
        ModConfig input = CreateValidConfig(config =>
        {
            config.EnableMod = false;
            config.EnableDebugMode = true;
        });

        Mock<IConfigReader> reader = CreateReaderReturning(input);
        ConfigLoader sut = new ConfigLoader(reader.Object);

        ModConfig result = sut.Load();

        Assert.NotSame(input, result);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
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

    private static Mock<IConfigReader> CreateReaderReturning(ModConfig config)
    {
        Mock<IConfigReader> reader = new Mock<IConfigReader>(MockBehavior.Strict);
        reader.Setup(x => x.ReadConfig<ModConfig>()).Returns(config);
        return reader;
    }
}
