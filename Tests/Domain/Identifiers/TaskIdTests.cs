using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests for TaskId invariants and deterministic equality behavior.
/// </summary>
public class TaskIdTests
{
    [Fact]
    public void Ctor_WhenValueHasOuterWhitespace_TrimsToCanonicalValue()
    {
        TaskId sut = new TaskId("  BuiltIn_ForageSweep  ");

        Assert.Equal("BuiltIn_ForageSweep", sut.Value);
    }

    [Fact]
    public void Ctor_WhenValueIsNull_ThrowsArgumentNullException()
    {
        string? raw = null;

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new TaskId(raw!));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WhenValueIsEmptyOrWhitespace_ThrowsArgumentException(string raw)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new TaskId(raw));

        Assert.Equal("identifier", exception.ParamName);
    }

    [Fact]
    public void Equals_WhenOnlyCaseDiffers_IsCaseSensitive()
    {
        TaskId first = new TaskId("BuiltIn_ForageSweep");
        TaskId second = new TaskId("builtin_foragesweep");

        Assert.NotEqual(first, second);
        Assert.False(first == second);
    }

    [Fact]
    public void Ctor_WhenValueIsNonWhitespaceWithoutCanonicalShape_PreservesCurrentBehavior()
    {
        TaskId sut = new TaskId("non-canonical-shape-is-currently-accepted");

        Assert.Equal("non-canonical-shape-is-currently-accepted", sut.Value);
    }
}
