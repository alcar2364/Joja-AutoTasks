using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests for DayKey canonical validation and deterministic equality behavior.
/// </summary>
public class DayKeyTests
{
    [Fact]
    public void Ctor_WhenValueHasOuterWhitespace_TrimsToCanonicalValue()
    {
        DayKey sut = new DayKey("  Year1-Summer15  ");

        Assert.Equal("Year1-Summer15", sut.Value);
    }

    [Fact]
    public void Ctor_WhenValueIsNull_ThrowsArgumentNullException()
    {
        string? raw = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new DayKey(raw!));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WhenValueIsEmptyOrWhitespace_ThrowsArgumentException(string raw)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new DayKey(raw));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Fact]
    public void Ctor_WhenFormatMissingDash_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new DayKey("Year1Summer15"));

        Assert.Equal("dayKey", exception.ParamName);
    }

    [Fact]
    public void Ctor_WhenSeasonTokenIsWrongCase_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new DayKey("Year1-summer15"));

        Assert.Equal("dayKey", exception.ParamName);
    }

    [Theory]
    [InlineData("Year0-Summer15")]
    [InlineData("Year-1-Summer15")]
    [InlineData("YearX-Summer15")]
    public void Ctor_WhenYearSegmentIsInvalid_ThrowsArgumentException(string raw)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new DayKey(raw));

        Assert.Equal("dayKey", exception.ParamName);
    }

    [Theory]
    [InlineData("Year1-Summer0")]
    [InlineData("Year1-Summer29")]
    [InlineData("Year1-SummerX")]
    public void Ctor_WhenDaySegmentIsInvalid_ThrowsArgumentException(string raw)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new DayKey(raw));

        Assert.Equal("dayKey", exception.ParamName);
    }

    [Fact]
    public void Equals_WhenCanonicalValuesMatchAfterTrimming_ReturnsTrue()
    {
        DayKey first = new DayKey("Year1-Summer15");
        DayKey second = new DayKey(" Year1-Summer15 ");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
