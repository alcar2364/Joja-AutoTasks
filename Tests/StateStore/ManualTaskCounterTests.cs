using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.StateStoreTests;

public sealed class ManualTaskCounterTests
{
    [Fact]
    public void ManualTaskCounter_IssueNextId_EmitsUniqueSequentialIdsDeterministically()
    {
        ManualTaskCounter firstCounter = new();
        ManualTaskCounter secondCounter = new();

        int[] firstSequence = Enumerable.Range(0, 8)
            .Select(_ => firstCounter.IssueNextId())
            .ToArray();

        int[] secondSequence = Enumerable.Range(0, 8)
            .Select(_ => secondCounter.IssueNextId())
            .ToArray();

        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7 }, firstSequence);
        Assert.Equal(firstSequence, secondSequence);
        Assert.Equal(firstSequence.Length, firstSequence.Distinct().Count());
    }

    [Fact]
    public void DispatchCreateManualTaskCommand_IssuesManualTaskIdsInSequenceWithoutCollisions()
    {
        Store stateStore = new();
        TaskSnapshot? latestSnapshot = null;
        DayKey creationDay = DayKeyFactory.Create(1, "Spring", 1);

        stateStore.SnapshotChanged += snapshot => latestSnapshot = snapshot;

        for (int issuedIndex = 0; issuedIndex < 6; issuedIndex++)
        {
            string title = $"Manual task {issuedIndex}";

            stateStore.DispatchCreateManualTaskCommand(
                category: TaskCategory.Farming,
                title: title,
                description: null,
                creationDay: creationDay);

            Assert.NotNull(latestSnapshot);

            TaskView createdTask = latestSnapshot!.TaskViews.Single(view => view.Title == title);
            Assert.Equal($"ManualTask_{issuedIndex}", createdTask.Id.Value);
        }

        Assert.NotNull(latestSnapshot);

        string[] issuedIds = latestSnapshot!.TaskViews
            .Select(view => view.Id.Value)
            .ToArray();

        Assert.Equal(issuedIds.Length, issuedIds.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void OnReturnToTitle_AfterManualIssuance_RestartsCounterDeterministically()
    {
        Store stateStore = new();
        TaskSnapshot? latestSnapshot = null;
        DayKey creationDay = DayKeyFactory.Create(1, "Spring", 1);

        stateStore.SnapshotChanged += snapshot => latestSnapshot = snapshot;

        stateStore.DispatchCreateManualTaskCommand(TaskCategory.Farming, "Before reset 0", null, creationDay);
        stateStore.DispatchCreateManualTaskCommand(TaskCategory.Farming, "Before reset 1", null, creationDay);

        stateStore.OnReturnToTitle();

        stateStore.DispatchCreateManualTaskCommand(TaskCategory.Farming, "After reset 0", null, creationDay);
        stateStore.DispatchCreateManualTaskCommand(TaskCategory.Farming, "After reset 1", null, creationDay);

        Assert.NotNull(latestSnapshot);

        TaskView firstAfterReset = latestSnapshot!.TaskViews.Single(view => view.Title == "After reset 0");
        TaskView secondAfterReset = latestSnapshot.TaskViews.Single(view => view.Title == "After reset 1");

        Assert.Equal("ManualTask_0", firstAfterReset.Id.Value);
        Assert.Equal("ManualTask_1", secondAfterReset.Id.Value);
        Assert.Equal(2, latestSnapshot.TaskViews.Count);
    }
}
