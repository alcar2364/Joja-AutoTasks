using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using Moq;
using StardewModdingAPI;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.StateStore;

public sealed class TimeChangedGatingTests
{
    [Fact]
    public void HandleTimeChanged_WhenNoSessionActive_IsIgnored()
    {
        RecordingEventDispatcher dispatcher = new();
        Store store = new();
        LifecycleCoordinator sut = CreateSut(dispatcher, store);
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);

        sut.HandleTimeChanged(day, 600);

        Assert.DoesNotContain(nameof(IEventDispatcher.DispatchTimeChanged), dispatcher.Calls);
    }

    [Fact]
    public void HandleTimeChanged_WhenSessionEndedAfterReturnedToTitle_IsIgnored()
    {
        RecordingEventDispatcher dispatcher = new();
        Store store = new();
        LifecycleCoordinator sut = CreateSut(dispatcher, store);
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);

        sut.HandleSaveLoaded(day, 600);
        sut.HandleReturnedToTitle();
        sut.HandleTimeChanged(day, 700);

        Assert.DoesNotContain(nameof(IEventDispatcher.DispatchTimeChanged), dispatcher.Calls);
    }

    [Fact]
    public void HandleTimeChanged_WhenSessionIsActive_IsForwarded()
    {
        RecordingEventDispatcher dispatcher = new();
        Store store = new();
        LifecycleCoordinator sut = CreateSut(dispatcher, store);
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);

        sut.HandleSaveLoaded(day, 600);
        sut.HandleTimeChanged(day, 700);

        Assert.Contains(nameof(IEventDispatcher.DispatchTimeChanged), dispatcher.Calls);
    }

    [Fact]
    public void OnTimeChanged_WhenSessionInactive_IsIgnored()
    {
        Store store = new();
        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.Debug);

        store.OnTimeChanged(DayKeyFactory.Create(1, "Spring", 1), 600);

        Assert.Throws<InvalidOperationException>(() =>
            store.Dispatch(CreateAddCommand(new TaskId("time_inactive_add"))));
    }

    [Fact]
    public void OnTimeChanged_WhenCalledWithIdenticalDayKeyAndTime_DoesNotRepublishSnapshot()
    {
        Store store = new();
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);
        int snapshotCount = 0;

        store.InitializeTimeContext(day, 600);
        store.OnSaveLoaded();
        store.SnapshotChanged += _ => snapshotCount++;

        store.OnTimeChanged(day, 600);
        store.OnTimeChanged(day, 600);

        Assert.Equal(0, snapshotCount);
    }

    [Fact]
    public void OnTimeChanged_WhenCalledWithNewTime_DoesNotPublishSnapshotByItself()
    {
        Store store = new();
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);
        int snapshotCount = 0;

        store.InitializeTimeContext(day, 600);
        store.OnSaveLoaded();
        store.SnapshotChanged += _ => snapshotCount++;

        store.OnTimeChanged(day, 700);

        Assert.Equal(0, snapshotCount);
    }

    [Fact]
    public void HandleTimeChanged_WhenEngineEmitsCommandAfterTimeChange_SnapshotChangedFires()
    {
        RecordingEventDispatcher dispatcher = new();
        Store store = new();
        LifecycleCoordinator sut = CreateSut(dispatcher, store);
        DayKey day = DayKeyFactory.Create(1, "Spring", 1);
        int snapshotCount = 0;
        TaskId taskId = new("time_change_then_command");

        sut.HandleSaveLoaded(day, 600);

        store.Dispatch(CreateAddCommand(taskId));
        snapshotCount = 0;
        store.SnapshotChanged += _ => snapshotCount++;

        sut.HandleTimeChanged(day, 700);
        Assert.Equal(0, snapshotCount);

        store.Dispatch(new CompleteTaskCommand(taskId, day, isPlayerInitiated: false));

        Assert.Equal(1, snapshotCount);
    }

    private static LifecycleCoordinator CreateSut(IEventDispatcher dispatcher, Store stateStore)
    {
        Mock<IMonitor> monitor = new(MockBehavior.Loose);
        ModLogger logger = new(monitor.Object);
        return new LifecycleCoordinator(logger, dispatcher, stateStore);
    }

    private static AddOrUpdateTaskCommand CreateAddCommand(TaskId taskId)
    {
        return new AddOrUpdateTaskCommand(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Time Changed Task",
            description: null,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            sourceIdentifier: "manual:test");
    }

    private sealed class RecordingEventDispatcher : IEventDispatcher
    {
        internal List<string> Calls { get; } = new();

        public void DispatchGameLaunched()
        {
            Calls.Add(nameof(DispatchGameLaunched));
        }

        public void DispatchSaveLoaded()
        {
            Calls.Add(nameof(DispatchSaveLoaded));
        }

        public void DispatchDayStarted()
        {
            Calls.Add(nameof(DispatchDayStarted));
        }

        public void DispatchReturnedToTitle()
        {
            Calls.Add(nameof(DispatchReturnedToTitle));
        }

        public void DispatchSavingInProgress()
        {
            Calls.Add(nameof(DispatchSavingInProgress));
        }

        public void DispatchTimeChanged(DayKey currentDay, int currentTime)
        {
            Calls.Add(nameof(DispatchTimeChanged));
        }

        public void DispatchUpdateTicked()
        {
            Calls.Add(nameof(DispatchUpdateTicked));
        }
    }
}