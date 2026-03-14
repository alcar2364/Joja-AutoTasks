using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Models;
using StateStoreType = JojaAutoTasks.State.StateStore;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Tests.StateStoreTests;

public sealed class SnapshotPublishingTests
{
    [Fact]
    public void Dispatch_WhenAddThenManualUpdateStateChanges_SnapshotChangedPublishesExpectedContentAndVersionProgression()
    {
        StateStoreType stateStore = new();
        List<TaskSnapshot> publishedSnapshots = new();
        TaskId taskId = new("Manual_SnapshotPublish_AddUpdate");

        stateStore.InitializeTimeContext(DayKeyFactory.Create(1, "Spring", 1), 600);
        stateStore.SnapshotChanged += snapshot => publishedSnapshots.Add(snapshot);

        stateStore.Dispatch(CreateManualAddOrUpdateCommand(
            taskId: taskId,
            title: "Collect eggs",
            description: "From coop",
            progressCurrent: 0,
            progressMax: 5,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            sourceIdentifier: "Manual:CollectEggs"));

        stateStore.Dispatch(CreateManualAddOrUpdateCommand(
            taskId: taskId,
            title: "Collect large eggs",
            description: "From upgraded coop",
            progressCurrent: 3,
            progressMax: 5,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            sourceIdentifier: "Manual:CollectEggs"));

        Assert.Equal(2, publishedSnapshots.Count);

        TaskSnapshot firstSnapshot = publishedSnapshots[0];
        Assert.Equal(1, firstSnapshot.Version);
        TaskView firstView = Assert.Single(firstSnapshot.TaskViews);
        Assert.Equal(taskId, firstView.Id);
        Assert.Equal("Collect eggs", firstView.Title);
        Assert.Equal("From coop", firstView.Description);
        Assert.Equal(0, firstView.ProgressCurrent);
        Assert.Equal(5, firstView.ProgressMax);
        Assert.Equal(TaskStatus.Incomplete, firstView.Status);

        TaskSnapshot secondSnapshot = publishedSnapshots[1];
        Assert.Equal(2, secondSnapshot.Version);
        TaskView secondView = Assert.Single(secondSnapshot.TaskViews);
        Assert.Equal(taskId, secondView.Id);
        Assert.Equal("Collect large eggs", secondView.Title);
        Assert.Equal("From upgraded coop", secondView.Description);
        Assert.Equal(0, secondView.ProgressCurrent);
        Assert.Equal(5, secondView.ProgressMax);
        Assert.Equal(TaskStatus.Incomplete, secondView.Status);
    }

    [Fact]
    public void Dispatch_WhenAddThenCompleteStateChanges_SnapshotChangedPublishesCompletedStateAndVersionProgression()
    {
        StateStoreType stateStore = new();
        List<TaskSnapshot> publishedSnapshots = new();
        TaskId taskId = new("Manual_SnapshotPublish_AddComplete");
        DayKey completionDay = DayKeyFactory.Create(1, "Spring", 3);

        stateStore.InitializeTimeContext(DayKeyFactory.Create(1, "Spring", 1), 600);
        stateStore.SnapshotChanged += snapshot => publishedSnapshots.Add(snapshot);

        stateStore.Dispatch(CreateManualAddOrUpdateCommand(
            taskId: taskId,
            title: "Water crops",
            description: null,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            sourceIdentifier: "Manual:WaterCrops"));

        stateStore.Dispatch(new CompleteTaskCommand(taskId, completionDay));

        Assert.Equal(2, publishedSnapshots.Count);

        TaskSnapshot firstSnapshot = publishedSnapshots[0];
        Assert.Equal(1, firstSnapshot.Version);
        TaskView firstView = Assert.Single(firstSnapshot.TaskViews);
        Assert.Equal(TaskStatus.Incomplete, firstView.Status);
        Assert.Null(firstView.CompletionDay);

        TaskSnapshot secondSnapshot = publishedSnapshots[1];
        Assert.Equal(2, secondSnapshot.Version);
        TaskView secondView = Assert.Single(secondSnapshot.TaskViews);
        Assert.Equal(taskId, secondView.Id);
        Assert.Equal("Water crops", secondView.Title);
        Assert.Equal(TaskStatus.Completed, secondView.Status);
        Assert.Equal(completionDay, secondView.CompletionDay);
    }

    [Fact]
    public void Dispatch_WhenCompleteTargetsMissingTask_DoesNotPublishSnapshot()
    {
        StateStoreType stateStore = new();
        int publishedCount = 0;

        stateStore.SnapshotChanged += _ => publishedCount++;

        stateStore.Dispatch(new CompleteTaskCommand(
            new TaskId("Manual_MissingTask_Complete"),
            DayKeyFactory.Create(1, "Spring", 5)));

        Assert.Equal(0, publishedCount);
    }

    [Fact]
    public void Dispatch_WhenRemoveTargetsMissingTask_DoesNotPublishSnapshot()
    {
        StateStoreType stateStore = new();
        int publishedCount = 0;

        stateStore.SnapshotChanged += _ => publishedCount++;

        stateStore.Dispatch(new RemoveTaskCommand(new TaskId("Manual_MissingTask_Remove")));

        Assert.Equal(0, publishedCount);
    }

    [Fact]
    public void Dispatch_WhenUncompleteTargetsMissingTask_DoesNotPublishSnapshot()
    {
        StateStoreType stateStore = new();
        int publishedCount = 0;

        stateStore.SnapshotChanged += _ => publishedCount++;

        stateStore.Dispatch(new UncompleteTaskCommand(new TaskId("Manual_MissingTask_Uncomplete")));

        Assert.Equal(0, publishedCount);
    }

    [Fact]
    public void Dispatch_WhenPinTargetsMissingTask_DoesNotPublishSnapshot()
    {
        StateStoreType stateStore = new();
        int publishedCount = 0;

        stateStore.SnapshotChanged += _ => publishedCount++;

        stateStore.Dispatch(new PinTaskCommand(new TaskId("Manual_MissingTask_Pin")));

        Assert.Equal(0, publishedCount);
    }

    [Fact]
    public void Dispatch_WhenUnpinTargetsMissingTask_DoesNotPublishSnapshot()
    {
        StateStoreType stateStore = new();
        int publishedCount = 0;

        stateStore.SnapshotChanged += _ => publishedCount++;

        stateStore.Dispatch(new UnpinTaskCommand(new TaskId("Manual_MissingTask_Unpin")));

        Assert.Equal(0, publishedCount);
    }

    private static AddOrUpdateTaskCommand CreateManualAddOrUpdateCommand(
        TaskId taskId,
        string title,
        string? description,
        int progressCurrent,
        int progressMax,
        DayKey creationDay,
        string sourceIdentifier)
    {
        return new AddOrUpdateTaskCommand(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: title,
            description: description,
            progressCurrent: progressCurrent,
            progressMax: progressMax,
            creationDay: creationDay,
            sourceIdentifier: sourceIdentifier);
    }
}
