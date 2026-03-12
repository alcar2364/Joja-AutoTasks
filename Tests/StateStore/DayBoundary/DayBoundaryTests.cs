using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.DayBoundary;
using JojaAutoTasks.State.Models;
using StateStoreType = JojaAutoTasks.State.StateStore;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Tests.StateStore.DayBoundary;

public sealed class DayBoundaryTests
{
    [Fact]
    public void DetectExpiredIds_WhenCurrentDayAdvancesAcrossSeasonsAndYears_ReturnsOnlyStrictlyOlderCreationDays()
    {
        StateContainer stateContainer = new();
        TaskId expiredFromPriorYear = new("manual_expired_prior_year");
        TaskId expiredFromPriorDay = new("manual_expired_prior_day");
        TaskId sameDay = new("manual_same_day");

        stateContainer.Set(expiredFromPriorYear, CreateTaskRecord(expiredFromPriorYear, DayKeyFactory.Create(1, "Winter", 28)));
        stateContainer.Set(expiredFromPriorDay, CreateTaskRecord(expiredFromPriorDay, DayKeyFactory.Create(2, "Spring", 1)));
        stateContainer.Set(sameDay, CreateTaskRecord(sameDay, DayKeyFactory.Create(2, "Spring", 2)));

        IReadOnlyList<TaskId> expiredIds = ExpirationDetector.DetectExpiredIds(
            stateContainer,
            DayKeyFactory.Create(2, "Spring", 2));

        Assert.Equal(2, expiredIds.Count);
        Assert.Contains(expiredFromPriorYear, expiredIds);
        Assert.Contains(expiredFromPriorDay, expiredIds);
        Assert.DoesNotContain(sameDay, expiredIds);
    }

    [Fact]
    public void RemoveExpiredTasks_WhenGivenExpiredIds_RemovesOnlySuppliedIds()
    {
        StateContainer stateContainer = new();
        TaskId expiredId = new("manual_expired_remove");
        TaskId activeId = new("manual_active_keep");

        stateContainer.Set(expiredId, CreateTaskRecord(expiredId, DayKeyFactory.Create(1, "Spring", 1)));
        stateContainer.Set(activeId, CreateTaskRecord(activeId, DayKeyFactory.Create(1, "Spring", 2)));

        DayTransitionHandler.RemoveExpiredTasks(new[] { expiredId }, stateContainer);

        Assert.False(stateContainer.TryGet(expiredId, out _));
        Assert.True(stateContainer.TryGet(activeId, out _));
    }

    [Fact]
    public void OnDayStarted_WhenExpiredTasksExist_RemovesThemAndPublishesSnapshotOnce()
    {
        StateStoreType stateStore = new();
        TaskId expiredId = new("manual_store_expired");
        TaskId activeId = new("manual_store_active");

        AddManualTask(stateStore, expiredId, DayKeyFactory.Create(1, "Spring", 1));
        AddManualTask(stateStore, activeId, DayKeyFactory.Create(1, "Spring", 2));

        int snapshotEventCount = 0;
        TaskSnapshot? publishedSnapshot = null;
        stateStore.SnapshotChanged += snapshot =>
        {
            snapshotEventCount++;
            publishedSnapshot = snapshot;
        };

        stateStore.OnDayStarted(DayKeyFactory.Create(1, "Spring", 2));

        Assert.Equal(1, snapshotEventCount);
        Assert.NotNull(publishedSnapshot);
        Assert.Single(publishedSnapshot.TaskViews);
        Assert.Equal(activeId, publishedSnapshot.TaskViews[0].Id);
    }

    [Fact]
    public void OnDayStarted_WhenNoExpiredTasksExist_DoesNotPublishSnapshot()
    {
        StateStoreType stateStore = new();
        TaskId activeId = new("manual_store_no_expiry");
        DayKey currentDay = DayKeyFactory.Create(1, "Spring", 4);

        AddManualTask(stateStore, activeId, currentDay);

        int snapshotEventCount = 0;
        stateStore.SnapshotChanged += _ => snapshotEventCount++;

        stateStore.OnDayStarted(currentDay);

        Assert.Equal(0, snapshotEventCount);
    }

    private static void AddManualTask(StateStoreType stateStore, TaskId taskId, DayKey creationDay)
    {
        AddOrUpdateTaskCommand command = new(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Task",
            description: null,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: creationDay,
            sourceIdentifier: "manual:test");

        stateStore.Dispatch(command);
    }

    private static TaskRecord CreateTaskRecord(TaskId id, DayKey creationDay)
    {
        return new TaskRecord(
            id: id,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Task",
            description: null,
            status: TaskStatus.Incomplete,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: creationDay,
            completionDay: null,
            sourceIdentifier: "manual:test",
            isPinned: false);
    }
}