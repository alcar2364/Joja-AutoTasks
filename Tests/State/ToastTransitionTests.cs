using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Models;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.StateStore;

public sealed class ToastTransitionTests
{
    [Fact]
    public void Dispatch_WhenEngineCompletesIncompleteTask_ToastRequestedFires()
    {
        Store store = new();
        TaskId taskId = new("toast_engine_complete");
        List<ToastEvent> toastEvents = new();
        List<TaskSnapshot> snapshots = new();

        InitializeAndSeedIncompleteTask(store, taskId, "Task");
        store.ToastRequested += toastEvents.Add;
        store.SnapshotChanged += snapshots.Add;

        store.Dispatch(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 1), isPlayerInitiated: false));

        Assert.Single(toastEvents);
        Assert.Equal(ToastType.TaskAutoCompleted, toastEvents[0].Type);
    }

    [Fact]
    public void Dispatch_WhenPlayerCompletesTask_ToastRequestedDoesNotFire()
    {
        Store store = new();
        TaskId taskId = new("toast_player_complete");
        List<ToastEvent> toastEvents = new();
        List<TaskSnapshot> snapshots = new();

        InitializeAndSeedIncompleteTask(store, taskId, "Task");
        store.ToastRequested += toastEvents.Add;
        store.SnapshotChanged += snapshots.Add;

        store.Dispatch(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 1), isPlayerInitiated: true));

        Assert.Empty(toastEvents);
    }

    [Fact]
    public void Dispatch_WhenTaskAlreadyCompleted_ToastRequestedDoesNotFire()
    {
        Store store = new();
        TaskId taskId = new("toast_already_complete");
        List<ToastEvent> toastEvents = new();
        List<TaskSnapshot> snapshots = new();

        InitializeAndSeedIncompleteTask(store, taskId, "Task");
        store.ToastRequested += toastEvents.Add;
        store.SnapshotChanged += snapshots.Add;

        store.Dispatch(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 1), isPlayerInitiated: false));
        store.Dispatch(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 1), isPlayerInitiated: false));

        Assert.Single(toastEvents);
    }

    [Fact]
    public void Dispatch_WhenEngineCompletesTask_ToastRequestedFiresBeforeSnapshotChanged()
    {
        Store store = new();
        TaskId taskId = new("toast_ordering");
        List<string> eventOrder = new();

        InitializeAndSeedIncompleteTask(store, taskId, "Task");
        store.ToastRequested += _ => eventOrder.Add("toast");
        store.SnapshotChanged += _ => eventOrder.Add("snapshot");

        store.Dispatch(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 1), isPlayerInitiated: false));

        Assert.Equal(2, eventOrder.Count);
        Assert.Equal("toast", eventOrder[0]);
        Assert.Equal("snapshot", eventOrder[1]);
    }

    [Fact]
    public void Dispatch_WhenEngineCompletesTask_ToastEventTaskTitleMatchesTaskTitle()
    {
        Store store = new();
        TaskId taskId = new("toast_title_match");
        List<ToastEvent> toastEvents = new();
        List<TaskSnapshot> snapshots = new();

        InitializeAndSeedIncompleteTask(store, taskId, "Harvest Parsnips");
        store.ToastRequested += toastEvents.Add;
        store.SnapshotChanged += snapshots.Add;

        store.Dispatch(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 1), isPlayerInitiated: false));

        ToastEvent toast = Assert.Single(toastEvents);
        Assert.Equal("Harvest Parsnips", toast.TaskTitle);
    }

    private static void InitializeAndSeedIncompleteTask(Store store, TaskId taskId, string title)
    {
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);
        store.InitializeTimeContext(day, 600);

        store.Dispatch(new AddOrUpdateTaskCommand(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: title,
            description: null,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: day,
            sourceIdentifier: "manual:test"));
    }
}