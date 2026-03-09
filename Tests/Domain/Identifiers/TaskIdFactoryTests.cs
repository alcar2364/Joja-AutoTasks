using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests deterministic and collision-safe TaskIdFactory behavior.
/// </summary>
public class TaskIdFactoryTests
{
    [Fact]
    public void CreateBuiltIn_WhenInputsRepeat_ReturnsEqualTaskIds()
    {
        DayKey dayKey = DayKeyFactory.Create(1, "Summer", 15);

        TaskId first = TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_route", dayKey);
        TaskId second = TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_route", dayKey);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void CreateTaskBuilder_WhenInputsRepeat_ReturnsEqualTaskIds()
    {
        DayKey dayKey = DayKeyFactory.Create(1, "Summer", 15);

        TaskId first = TaskIdFactory.CreateTaskBuilder("Rule-Alpha", "forest_route", dayKey);
        TaskId second = TaskIdFactory.CreateTaskBuilder("Rule-Alpha", "forest_route", dayKey);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void CreateBuiltInAndTaskBuilder_WhenComponentsMatch_DoNotCollide()
    {
        DayKey dayKey = DayKeyFactory.Create(2, "Fall", 8);

        TaskId builtIn = TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_route", dayKey);
        TaskId taskBuilder = TaskIdFactory.CreateTaskBuilder("ForageSweep", "forest_route", dayKey);

        Assert.NotEqual(builtIn, taskBuilder);
        Assert.StartsWith("BuiltIn_", builtIn.Value);
        Assert.StartsWith("TaskBuilder_", taskBuilder.Value);
    }

    [Fact]
    public void CreateBuiltIn_WhenOptionalSegmentsMissing_UsesCanonicalTwoSegmentShape()
    {
        TaskId sut = TaskIdFactory.CreateBuiltIn("ForageSweep");

        Assert.Equal("BuiltIn_ForageSweep", sut.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateBuiltIn_WhenGeneratorIdIsInvalid_ThrowsArgumentException(string generatorId)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            TaskIdFactory.CreateBuiltIn(generatorId));

        Assert.Equal("generatorId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateTaskBuilder_WhenRuleIdIsInvalid_ThrowsArgumentException(string ruleId)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            TaskIdFactory.CreateTaskBuilder(ruleId));

        Assert.Equal("ruleId", exception.ParamName);
    }
}
