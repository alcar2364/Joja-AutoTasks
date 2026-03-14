using System.Collections.Generic;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Tests.StateStore.Models;

public class SnapshotImmutabilityTests
{
    [Fact]
    public void Project_WhenCanonicalRecordReferenceIsMutatedAfterProjection_SnapshotRemainsDefensiveCopy()
    {
        StateContainer state = new();
        TaskId taskId = new("manual_snapshot_copy");
        TaskRecord canonicalRecord = CreateRecord(taskId, "Original title", "Original description", 1);

        state.Set(taskId, canonicalRecord);

        TaskSnapshot snapshot = SnapshotProjector.Project(state, DayKeyFactory.Create(1, "Spring", 1), 600);

        canonicalRecord.Title = "Mutated title";
        canonicalRecord.Description = "Mutated description";
        canonicalRecord.ProgressCurrent = 4;

        TaskView projected = Assert.Single(snapshot.TaskViews);
        Assert.Equal("Original title", projected.Title);
        Assert.Equal("Original description", projected.Description);
        Assert.Equal(1, projected.ProgressCurrent);
    }

    [Fact]
    public void Project_WhenCanonicalStateChangesAfterCapture_PreviouslyCapturedSnapshotDoesNotChange()
    {
        StateContainer state = new();
        TaskId taskId = new("manual_snapshot_stability");

        state.Set(taskId, CreateRecord(taskId, "Before update", null, 0));
        TaskSnapshot firstSnapshot = SnapshotProjector.Project(state, DayKeyFactory.Create(1, "Spring", 1), 600);

        state.Set(taskId, CreateRecord(taskId, "After update", "new description", 5));
        TaskSnapshot secondSnapshot = SnapshotProjector.Project(state, DayKeyFactory.Create(1, "Spring", 1), 600);

        TaskView firstProjected = Assert.Single(firstSnapshot.TaskViews);
        TaskView secondProjected = Assert.Single(secondSnapshot.TaskViews);

        Assert.Equal("Before update", firstProjected.Title);
        Assert.Null(firstProjected.Description);
        Assert.Equal(0, firstProjected.ProgressCurrent);

        Assert.Equal("After update", secondProjected.Title);
        Assert.Equal("new description", secondProjected.Description);
        Assert.Equal(5, secondProjected.ProgressCurrent);
    }

    [Fact]
    public void Project_TaskViewsCollection_ExposesReadOnlyBehavior()
    {
        StateContainer state = new();
        TaskId taskId = new("manual_snapshot_readonly");

        state.Set(taskId, CreateRecord(taskId, "Immutable list", null, 2));
        TaskSnapshot snapshot = SnapshotProjector.Project(state, DayKeyFactory.Create(1, "Spring", 1), 600);

        Assert.IsAssignableFrom<IReadOnlyList<TaskView>>(snapshot.TaskViews);

        IList<TaskView>? writableList = snapshot.TaskViews as IList<TaskView>;
        Assert.NotNull(writableList);

        Assert.Throws<NotSupportedException>(() => writableList.Add(CreateView(new TaskId("manual_snapshot_readonly_extra"))));

        Assert.Single(snapshot.TaskViews);
        Assert.Single(state.GetAll());
    }

    private static TaskRecord CreateRecord(TaskId id, string title, string? description, int progressCurrent)
    {
        return new TaskRecord(
            id: id,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: title,
            description: description,
            status: TaskStatus.Incomplete,
            progressCurrent: progressCurrent,
            progressMax: 10,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            completionDay: null,
            sourceIdentifier: "manual:test",
            isPinned: false);
    }

    private static TaskView CreateView(TaskId id)
    {
        return new TaskView(
            id: id,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Injected",
            description: null,
            status: TaskStatus.Incomplete,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            completionDay: null,
            sourceIdentifier: "manual:injected",
            isPinned: false);
    }
}
