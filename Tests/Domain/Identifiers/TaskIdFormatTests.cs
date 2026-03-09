using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>Tests underscore-preserving parsing behavior in <see cref="TaskIdFormat" />.</summary>
public class TaskIdFormatTests
{
    // -- Public API -- //
    [Fact]
    public void TryParse_WhenBuiltInSubjectContainsUnderscoresAndDayKey_RoundTripsSuccessfully()
    {
        DayKey dayKey = DayKeyFactory.Create(1, "Summer", 15);
        TaskId original = TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_dropoff_route", dayKey);

        bool parsed = TaskIdFormat.TryParse(original.Value, out TaskId reparsed);

        Assert.True(parsed);
        Assert.Equal(original, reparsed);
    }

    [Fact]
    public void TryParse_WhenTaskBuilderSubjectContainsUnderscores_RoundTripsSuccessfully()
    {
        TaskId original = TaskIdFactory.CreateTaskBuilder("RuleA", "barn_animal_feed_slot");

        bool parsed = TaskIdFormat.TryParse(original.Value, out TaskId reparsed);

        Assert.True(parsed);
        Assert.Equal(original, reparsed);
    }

    [Fact]
    public void TryParse_WhenBuiltInGeneratorSegmentMissing_ReturnsFalse()
    {
        const string rawId = "BuiltIn__forest_dropoff_route_Year1-Summer15";

        bool parsed = TaskIdFormat.TryParse(rawId, out TaskId _);

        Assert.False(parsed);
    }

    [Fact]
    public void TryParse_WhenManualCounterIsNonNegative_RoundTripsSuccessfully()
    {
        TaskId original = new TaskId("Manual_42");

        bool parsed = TaskIdFormat.TryParse(original.Value, out TaskId reparsed);

        Assert.True(parsed);
        Assert.Equal(original, reparsed);
    }

    [Fact]
    public void TryParse_WhenManualCounterIsNegative_ReturnsFalse()
    {
        const string rawId = "Manual_-1";

        bool parsed = TaskIdFormat.TryParse(rawId, out TaskId _);

        Assert.False(parsed);
    }

    [Fact]
    public void TryParse_WhenManualShapeContainsExtraSegments_ReturnsFalse()
    {
        const string rawId = "Manual_42_extra";

        bool parsed = TaskIdFormat.TryParse(rawId, out TaskId _);

        Assert.False(parsed);
    }

    [Fact]
    public void TryParse_WhenRawIdHasOuterWhitespace_ReturnsFalse()
    {
        const string rawId = "  BuiltIn_ForageSweep  ";

        bool parsed = TaskIdFormat.TryParse(rawId, out TaskId _);

        Assert.False(parsed);
    }
}
