using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.State.Models;
using Moq;
using StardewModdingAPI;
using Xunit;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.Lifecycle;

/// <summary>
/// Verifies integration between lifecycle forwarding and state-store init/teardown hooks.
/// </summary>
public class LifecycleCoordinatorIntegrationTests
{
    [Fact]
    public void HandleSaveLoaded_InvokesStateStorePathSafely_AndPreservesDispatcherForwarding()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();
        Store stateStore = new Store();
        LifecycleCoordinator sut = CreateSut(dispatcher, stateStore);

        DayKey day = DayKeyFactory.Create(1, "Spring", 1);
        stateStore.DispatchCreateManualTaskCommand(
            category: TaskCategory.Farming,
            title: "Before save-load",
            description: null,
            creationDay: day);

        int publishedCount = 0;
        TaskSnapshot? latestSnapshot = null;
        stateStore.SnapshotChanged += snapshot =>
        {
            publishedCount++;
            latestSnapshot = snapshot;
        };

        Exception? captured = Record.Exception(() => sut.HandleSaveLoaded());

        Assert.Null(captured);
        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchSaveLoaded), dispatcher.Calls[0]);
        Assert.Equal(0, publishedCount);

        stateStore.DispatchCreateManualTaskCommand(
            category: TaskCategory.Farming,
            title: "After save-load",
            description: null,
            creationDay: day);

        Assert.Equal(1, publishedCount);
        Assert.NotNull(latestSnapshot);
        Assert.Equal(2, latestSnapshot!.TaskViews.Count);
    }

    [Fact]
    public void HandleReturnedToTitle_ForwardsDispatcherAndResetsStateStoreForNextSession()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();
        Store stateStore = new Store();
        LifecycleCoordinator sut = CreateSut(dispatcher, stateStore);

        TaskSnapshot? beforeTeardownSnapshot = null;
        TaskSnapshot? afterTeardownSnapshot = null;
        int snapshotEventCount = 0;

        stateStore.SnapshotChanged += snapshot =>
        {
            snapshotEventCount++;
            if (snapshotEventCount == 1)
            {
                beforeTeardownSnapshot = snapshot;
            }
            else if (snapshotEventCount == 2)
            {
                afterTeardownSnapshot = snapshot;
            }
        };

        DayKey day = DayKeyFactory.Create(1, "Spring", 1);

        stateStore.DispatchCreateManualTaskCommand(
            category: TaskCategory.Farming,
            title: "Pre-title task",
            description: "Seed state before teardown",
            creationDay: day);

        sut.HandleReturnedToTitle();

        Assert.Equal(1, snapshotEventCount);

        stateStore.DispatchCreateManualTaskCommand(
            category: TaskCategory.Farming,
            title: "Post-title task",
            description: "Fresh state after teardown",
            creationDay: day);

        Assert.Equal(nameof(IEventDispatcher.DispatchReturnedToTitle), Assert.Single(dispatcher.Calls));

        Assert.NotNull(beforeTeardownSnapshot);
        Assert.NotNull(afterTeardownSnapshot);

        TaskView preTeardownTask = Assert.Single(beforeTeardownSnapshot!.TaskViews);
        TaskView postTeardownTask = Assert.Single(afterTeardownSnapshot!.TaskViews);

        Assert.Equal("Pre-title task", preTeardownTask.Title);
        Assert.Equal("Post-title task", postTeardownTask.Title);
        Assert.Equal(preTeardownTask.Id, postTeardownTask.Id);
    }

    private static LifecycleCoordinator CreateSut(IEventDispatcher dispatcher, Store stateStore)
    {
        Mock<IMonitor> monitor = new(MockBehavior.Loose);
        ModLogger logger = new(monitor.Object);
        return new LifecycleCoordinator(logger, dispatcher, stateStore);
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

        public void DispatchUpdateTicked()
        {
            Calls.Add(nameof(DispatchUpdateTicked));
        }
    }
}
