using JojaAutoTasks.Configuration;
using Moq;
using Xunit;

namespace JojaAutoTasks.Tests.Configuration;

/// <summary>Verifies version normalization keeps non-version configuration settings stable.</summary>
public class ConfigLoaderMigrationSafetyTests
{
    public static TheoryData<int> OlderVersions => new TheoryData<int>
    {
        ModConfig.CurrentConfigVersion - 1,
        ModConfig.CurrentConfigVersion - 10,
        int.MinValue
    };

    public static TheoryData<int> FutureVersions => new TheoryData<int>
    {
        ModConfig.CurrentConfigVersion + 1,
        ModConfig.CurrentConfigVersion + 10,
        int.MaxValue
    };

    [Theory]
    [MemberData(nameof(OlderVersions))]
    public void Load_WhenVersionIsOlder_NormalizesToCurrentVersion(int olderVersion)
    {
        ModConfig input = CreateConfig(olderVersion, enableMod: false, enableDebugMode: true);
        ConfigLoader sut = CreateSutReturning(input, out Mock<IConfigReader> reader);

        ModConfig result = sut.Load();

        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        Assert.NotSame(input, result);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Theory]
    [MemberData(nameof(FutureVersions))]
    public void Load_WhenVersionIsFuture_NormalizesToCurrentVersion(int futureVersion)
    {
        ModConfig input = CreateConfig(futureVersion, enableMod: false, enableDebugMode: true);
        ConfigLoader sut = CreateSutReturning(input, out Mock<IConfigReader> reader);

        ModConfig result = sut.Load();

        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        Assert.NotSame(input, result);
        reader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenVersionDiffersFromCurrent_KeepsNonVersionSettingsStable()
    {
        ModConfig olderInput = CreateConfig(ModConfig.CurrentConfigVersion - 1, enableMod: false, enableDebugMode: true);
        ModConfig futureInput = CreateConfig(ModConfig.CurrentConfigVersion + 1, enableMod: false, enableDebugMode: true);

        ConfigLoader olderSut = CreateSutReturning(olderInput, out Mock<IConfigReader> olderReader);
        ConfigLoader futureSut = CreateSutReturning(futureInput, out Mock<IConfigReader> futureReader);

        ModConfig fromOlder = olderSut.Load();
        ModConfig fromFuture = futureSut.Load();

        Assert.Equal(ModConfig.CurrentConfigVersion, fromOlder.ConfigVersion);
        Assert.Equal(ModConfig.CurrentConfigVersion, fromFuture.ConfigVersion);
        Assert.Equal(fromOlder.EnableMod, fromFuture.EnableMod);
        Assert.Equal(fromOlder.EnableDebugMode, fromFuture.EnableDebugMode);

        olderReader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
        futureReader.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    private static ModConfig CreateConfig(int configVersion, bool enableMod, bool enableDebugMode)
    {
        return new ModConfig
        {
            ConfigVersion = configVersion,
            EnableMod = enableMod,
            EnableDebugMode = enableDebugMode
        };
    }

    private static ConfigLoader CreateSutReturning(ModConfig config, out Mock<IConfigReader> reader)
    {
        reader = new Mock<IConfigReader>(MockBehavior.Strict);
        reader.Setup(x => x.ReadConfig<ModConfig>()).Returns(config);

        return new ConfigLoader(reader.Object);
    }
}
