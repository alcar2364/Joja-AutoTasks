using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests deterministic DayKeyFactory output and guard behavior.
/// </summary>
public class DayKeyFactoryTests
{
    [Fact]
    public void Create_WhenInputsAreCanonical_ReturnsCanonicalDayKey()
    {
        DayKey sut = DayKeyFactory.Create(1, "Summer", 15);

        Assert.Equal("Year1-Summer15", sut.Value);
    }

    [Fact]
    public void Create_WhenInputsRepeat_ReturnsDeterministicValue()
    {
        DayKey first = DayKeyFactory.Create(2, "Fall", 8);
        DayKey second = DayKeyFactory.Create(2, "Fall", 8);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenSeasonTokenIsNullOrWhitespace_ThrowsArgumentException(string? seasonToken)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            DayKeyFactory.Create(1, seasonToken!, 1));

        Assert.Equal("seasonToken", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WhenYearIsInvalid_ThrowsArgumentException(int year)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            DayKeyFactory.Create(year, "Summer", 1));

        Assert.Equal("dayKey", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(29)]
    public void Create_WhenDayIsInvalid_ThrowsArgumentException(int day)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            DayKeyFactory.Create(1, "Summer", day));

        Assert.Equal("dayKey", exception.ParamName);
    }

    [Theory]
    [InlineData("summer")]
    [InlineData("SPRING")]
    [InlineData("Autumn")]
    public void Create_WhenSeasonTokenIsNotCanonical_ThrowsArgumentException(string seasonToken)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            DayKeyFactory.Create(1, seasonToken, 1));

        Assert.Equal("dayKey", exception.ParamName);
    }
}
