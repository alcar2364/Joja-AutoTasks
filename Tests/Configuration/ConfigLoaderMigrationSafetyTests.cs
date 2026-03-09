using JojaAutoTasks.Configuration;
using Moq;
using StardewModdingAPI;
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
        ConfigLoader sut = CreateSutReturning(input, out Mock<IModHelper> helper);

        ModConfig result = sut.Load();

        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        Assert.NotSame(input, result);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Theory]
    [MemberData(nameof(FutureVersions))]
    public void Load_WhenVersionIsFuture_NormalizesToCurrentVersion(int futureVersion)
    {
        ModConfig input = CreateConfig(futureVersion, enableMod: false, enableDebugMode: true);
        ConfigLoader sut = CreateSutReturning(input, out Mock<IModHelper> helper);

        ModConfig result = sut.Load();

        Assert.Equal(ModConfig.CurrentConfigVersion, result.ConfigVersion);
        Assert.False(result.EnableMod);
        Assert.True(result.EnableDebugMode);
        Assert.NotSame(input, result);
        helper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
    }

    [Fact]
    public void Load_WhenVersionDiffersFromCurrent_KeepsNonVersionSettingsStable()
    {
        ModConfig olderInput = CreateConfig(ModConfig.CurrentConfigVersion - 1, enableMod: false, enableDebugMode: true);
        ModConfig futureInput = CreateConfig(ModConfig.CurrentConfigVersion + 1, enableMod: false, enableDebugMode: true);

        ConfigLoader olderSut = CreateSutReturning(olderInput, out Mock<IModHelper> olderHelper);
        ConfigLoader futureSut = CreateSutReturning(futureInput, out Mock<IModHelper> futureHelper);

        ModConfig fromOlder = olderSut.Load();
        ModConfig fromFuture = futureSut.Load();

        Assert.Equal(ModConfig.CurrentConfigVersion, fromOlder.ConfigVersion);
        Assert.Equal(ModConfig.CurrentConfigVersion, fromFuture.ConfigVersion);
        Assert.Equal(fromOlder.EnableMod, fromFuture.EnableMod);
        Assert.Equal(fromOlder.EnableDebugMode, fromFuture.EnableDebugMode);

        olderHelper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
        futureHelper.Verify(x => x.ReadConfig<ModConfig>(), Times.Once);
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

    private static ConfigLoader CreateSutReturning(ModConfig config, out Mock<IModHelper> helper)
    {
        helper = new Mock<IModHelper>(MockBehavior.Strict);
        helper.Setup(x => x.ReadConfig<ModConfig>()).Returns(config);

        return new ConfigLoader(helper.Object);
    }
}
