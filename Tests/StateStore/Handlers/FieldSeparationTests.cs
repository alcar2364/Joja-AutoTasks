using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Handlers;
using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Tests.StateStore.Handlers;

public class FieldSeparationTests
{
    [Fact]
    public void Handle_WhenEngineUpdateTargetsExistingManualRecord_PreservesUserPinState()
    {
        StateContainer state = new();
        AddOrUpdateTaskCommandHandler sut = new();
        TaskId taskId = TaskIdFactory.CreateManual(7);
        DayKey creationDay = DayKeyFactory.Create(1, "Spring", 5);
        DayKey completionDay = DayKeyFactory.Create(1, "Spring", 6);

        TaskRecord existingRecord = new(
            id: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Original title",
            description: "Original description",
            status: TaskStatus.Completed,
            progressCurrent: 3,
            progressMax: 5,
            creationDay: creationDay,
            completionDay: completionDay,
            sourceIdentifier: "Manual.7",
            isPinned: true);

        state.Set(taskId, existingRecord);

        AddOrUpdateTaskCommand command = new(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.BuiltIn,
            title: "Updated title",
            description: "Updated description",
            progressCurrent: 4,
            progressMax: 5,
            creationDay: creationDay,
            sourceIdentifier: "Manual.7");

        sut.Handle(command, state);

        bool exists = state.TryGet(taskId, out TaskRecord? updatedRecord);

        Assert.True(exists);
        Assert.NotNull(updatedRecord);
        Assert.True(updatedRecord.IsPinned);
        Assert.Equal(4, updatedRecord.ProgressCurrent);
        Assert.Equal(TaskStatus.Completed, updatedRecord.Status);
    }

    [Fact]
    public void Handle_WhenManualRecordIsUpdated_PreservesExistingEngineProgressCurrent()
    {
        StateContainer state = new();
        AddOrUpdateTaskCommandHandler sut = new();
        TaskId taskId = TaskIdFactory.CreateManual(11);
        DayKey creationDay = DayKeyFactory.Create(1, "Summer", 1);

        TaskRecord existingRecord = new(
            id: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Original title",
            description: "Original description",
            status: TaskStatus.Incomplete,
            progressCurrent: 1,
            progressMax: 10,
            creationDay: creationDay,
            completionDay: null,
            sourceIdentifier: "Manual.11",
            isPinned: false);

        state.Set(taskId, existingRecord);

        AddOrUpdateTaskCommand command = new(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "User edited title",
            description: "User edited description",
            progressCurrent: 6,
            progressMax: 10,
            creationDay: creationDay,
            sourceIdentifier: "Manual.11");

        sut.Handle(command, state);

        bool exists = state.TryGet(taskId, out TaskRecord? updatedRecord);

        Assert.True(exists);
        Assert.NotNull(updatedRecord);
        Assert.Equal(1, updatedRecord.ProgressCurrent);
        Assert.Equal(10, updatedRecord.ProgressMax);
    }

    [Fact]
    public void Handle_WhenEngineUpdateTargetsExistingNonManualRecord_UpdatesEngineFieldsAndPreservesUserFields()
    {
        StateContainer state = new();
        AddOrUpdateTaskCommandHandler sut = new();
        TaskId taskId = TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_route", DayKeyFactory.Create(1, "Summer", 7));
        DayKey creationDay = DayKeyFactory.Create(1, "Summer", 7);

        TaskRecord existingRecord = new(
            id: taskId,
            category: TaskCategory.Exploration,
            sourceType: TaskSourceType.BuiltIn,
            title: "Original built-in title",
            description: "Original built-in description",
            status: TaskStatus.Incomplete,
            progressCurrent: 2,
            progressMax: 5,
            creationDay: creationDay,
            completionDay: null,
            sourceIdentifier: "BuiltIn.Forager",
            isPinned: true);

        state.Set(taskId, existingRecord);
        long versionBeforeUpdateAttempt = state.Version;

        AddOrUpdateTaskCommand command = new(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.BuiltIn,
            title: "Updated built-in title",
            description: "Updated built-in description",
            progressCurrent: 99,
            progressMax: 99,
            creationDay: creationDay,
            sourceIdentifier: "BuiltIn.Forager.Updated");

        sut.Handle(command, state);

        bool exists = state.TryGet(taskId, out TaskRecord? updatedRecord);

        Assert.True(exists);
        Assert.NotNull(updatedRecord);
        Assert.Equal("Updated built-in title", updatedRecord.Title);
        Assert.Equal("Updated built-in description", updatedRecord.Description);
        Assert.Equal(99, updatedRecord.ProgressCurrent);
        Assert.Equal(99, updatedRecord.ProgressMax);
        Assert.True(updatedRecord.IsPinned);
        Assert.True(state.Version > versionBeforeUpdateAttempt);
    }
}

