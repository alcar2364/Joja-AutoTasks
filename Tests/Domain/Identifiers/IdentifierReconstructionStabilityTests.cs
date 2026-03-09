using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.Tests.Domain.Identifiers;

/// <summary>
/// Tests reconstruction seam stability for TaskId and DayKey values.
/// </summary>
public class IdentifierReconstructionStabilityTests
{
    [Fact]
    public void TaskId_WhenReconstructedFromCanonicalString_RemainsEqualWithStableHash()
    {
        TaskId original = TaskIdFactory.CreateTaskBuilder("Rule-A", "barn_slot_1", DayKeyFactory.Create(2, "Fall", 14));
        TaskId reconstructed = new TaskId(TaskIdFormat.Format(original));

        Assert.Equal(original, reconstructed);
        Assert.Equal(original.GetHashCode(), reconstructed.GetHashCode());
    }

    [Fact]
    public void TaskId_WhenReconstructedThroughParser_RemainsEqualWithStableHash()
    {
        TaskId original = TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_route", DayKeyFactory.Create(1, "Summer", 15));

        bool parsed = TaskIdFormat.TryParse(original.Value, out TaskId reconstructed);

        Assert.True(parsed);
        Assert.Equal(original, reconstructed);
        Assert.Equal(original.GetHashCode(), reconstructed.GetHashCode());
    }

    [Fact]
    public void DayKey_WhenReconstructedFromCanonicalString_RemainsEqualWithStableHash()
    {
        DayKey original = DayKeyFactory.Create(3, "Winter", 7);
        DayKey reconstructed = new DayKey(original.Value);

        Assert.Equal(original, reconstructed);
        Assert.Equal(original.GetHashCode(), reconstructed.GetHashCode());
    }

    [Fact]
    public void TaskIds_WhenReconstructedAcrossCtorAndParser_HaveStableOrdinalOrdering()
    {
        TaskId[] original =
        {
            TaskIdFactory.CreateBuiltIn("A", "one", DayKeyFactory.Create(1, "Spring", 1)),
            TaskIdFactory.CreateTaskBuilder("Rule-1", "subject", DayKeyFactory.Create(1, "Spring", 2)),
            new TaskId("Manual_2"),
            new TaskId("Manual_10")
        };

        TaskId[] ctorReconstructed = original
            .Select(taskId => new TaskId(taskId.Value))
            .ToArray();

        TaskId[] parserReconstructed = original
            .Select(taskId =>
            {
                bool parsed = TaskIdFormat.TryParse(taskId.Value, out TaskId parsedTaskId);
                Assert.True(parsed);
                return parsedTaskId;
            })
            .ToArray();

        string[] originalOrdered = original
            .OrderBy(taskId => taskId.Value, StringComparer.Ordinal)
            .Select(taskId => taskId.Value)
            .ToArray();

        string[] ctorOrdered = ctorReconstructed
            .OrderBy(taskId => taskId.Value, StringComparer.Ordinal)
            .Select(taskId => taskId.Value)
            .ToArray();

        string[] parserOrdered = parserReconstructed
            .OrderBy(taskId => taskId.Value, StringComparer.Ordinal)
            .Select(taskId => taskId.Value)
            .ToArray();

        Assert.Equal(originalOrdered, ctorOrdered);
        Assert.Equal(originalOrdered, parserOrdered);
    }
}
