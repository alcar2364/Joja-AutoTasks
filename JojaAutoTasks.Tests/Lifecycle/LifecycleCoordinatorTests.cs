using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using Moq;
using StardewModdingAPI;
using Xunit;

namespace JojaAutoTasks.Tests.Lifecycle;

public class LifecycleCoordinatorTests
{
    [Fact]
    public void HandleGameLaunched_ForwardsSignalToDispatcher()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();

        LifecycleCoordinator sut = CreateSut(dispatcher);

        sut.HandleGameLaunched();

        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchGameLaunched), dispatcher.Calls[0]);
    }

    [Fact]
    public void HandleSaveLoaded_ForwardsSignalToDispatcher()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();

        LifecycleCoordinator sut = CreateSut(dispatcher);

        sut.HandleSaveLoaded();

        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchSaveLoaded), dispatcher.Calls[0]);
    }

    [Fact]
    public void HandleSavingInProgress_ForwardsSignalOnlyToDispatcher()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();

        LifecycleCoordinator sut = CreateSut(dispatcher);

        sut.HandleSavingInProgress();

        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchSavingInProgress), dispatcher.Calls[0]);
    }

    [Fact]
    public void HandleUpdateTicked_ForwardsSignalToDispatcher_WhenDebugModeDisabled()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();

        LifecycleCoordinator sut = CreateSut(dispatcher);

        sut.HandleUpdateTicked(isDebugMode: false);

        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchUpdateTicked), dispatcher.Calls[0]);
    }

    [Fact]
    public void HandleGameLaunchedThenSaveLoaded_PreservesDispatchOrder()
    {
        RecordingEventDispatcher dispatcher = new RecordingEventDispatcher();

        LifecycleCoordinator sut = CreateSut(dispatcher);

        sut.HandleGameLaunched();
        sut.HandleSaveLoaded();

        Assert.Equal(2, dispatcher.Calls.Count);
        Assert.Equal(nameof(IEventDispatcher.DispatchGameLaunched), dispatcher.Calls[0]);
        Assert.Equal(nameof(IEventDispatcher.DispatchSaveLoaded), dispatcher.Calls[1]);
    }

    private static LifecycleCoordinator CreateSut(IEventDispatcher eventDispatcher)
    {
        Mock<IMonitor> monitor = new Mock<IMonitor>(MockBehavior.Loose);
        ModLogger logger = new ModLogger(monitor.Object);
        return new LifecycleCoordinator(logger, eventDispatcher);
    }

    private sealed class RecordingEventDispatcher : IEventDispatcher
    {
        internal List<string> Calls { get; } = new List<string>();

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
