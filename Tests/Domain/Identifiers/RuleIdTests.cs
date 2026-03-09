using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests for RuleId value-object invariants.
/// </summary>
public class RuleIdTests
{
    [Fact]
    public void Ctor_WhenValueHasOuterWhitespace_TrimsToCanonicalValue()
    {
        RuleId sut = new RuleId("  Rule-Alpha  ");

        Assert.Equal("Rule-Alpha", sut.Value);
    }

    [Fact]
    public void Ctor_WhenValueIsNull_ThrowsArgumentNullException()
    {
        string? raw = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new RuleId(raw!));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WhenValueIsEmptyOrWhitespace_ThrowsArgumentException(string raw)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new RuleId(raw));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Fact]
    public void Equals_WhenOnlyCaseDiffers_IsCaseSensitive()
    {
        RuleId first = new RuleId("Rule-Alpha");
        RuleId second = new RuleId("rule-alpha");

        Assert.NotEqual(first, second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equals_WhenCanonicalValuesMatchAfterTrimming_ReturnsTrue()
    {
        RuleId first = new RuleId("Rule-Alpha");
        RuleId second = new RuleId("  Rule-Alpha  ");

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
