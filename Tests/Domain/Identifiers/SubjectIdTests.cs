using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests for SubjectId value-object invariants.
/// </summary>
public class SubjectIdTests
{
    [Fact]
    public void Ctor_WhenValueHasOuterWhitespace_TrimsToCanonicalValue()
    {
        SubjectId sut = new SubjectId("  Barn_Animal_001  ");

        Assert.Equal("Barn_Animal_001", sut.Value);
    }

    [Fact]
    public void Ctor_WhenValueIsNull_ThrowsArgumentNullException()
    {
        string? raw = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new SubjectId(raw!));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WhenValueIsEmptyOrWhitespace_ThrowsArgumentException(string raw)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new SubjectId(raw));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Fact]
    public void Equals_WhenOnlyCaseDiffers_IsCaseSensitive()
    {
        SubjectId first = new SubjectId("Barn_Animal_001");
        SubjectId second = new SubjectId("barn_animal_001");

        Assert.NotEqual(first, second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equals_WhenCanonicalValuesMatchAfterTrimming_ReturnsTrue()
    {
        SubjectId first = new SubjectId("Barn_Animal_001");
        SubjectId second = new SubjectId("  Barn_Animal_001  ");

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
