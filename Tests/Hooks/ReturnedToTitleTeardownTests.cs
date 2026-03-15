using System.Reflection;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.Startup;
using JojaAutoTasks.Ui;
using Moq;
using StardewModdingAPI;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.Hooks;

/// <summary>Tests return-to-title teardown ordering through <see cref="ModEntry" />.</summary>
public sealed class ReturnedToTitleTeardownTests
{
    [Fact]
    public void OnReturnedToTitle_DisposesHudLifecycleBeforeRuntimeTeardownClearsState()
    {
        Store stateStore = new();
        RecordingEventDispatcher dispatcher = new(stateStore);
        ModRuntime runtime = CreateRuntime(dispatcher, stateStore);
        ModEntry sut = CreateEntryWithRuntime(runtime);
        dispatcher.AttachEntry(sut);

        DayKey day = DayKeyFactory.Create(1, "Spring", 1);
        runtime.LifecycleCoordinator.HandleSaveLoaded(day, 600);
        SetHudViewModel(sut, HudViewModel.ReplaceCurrent(null));
        stateStore.DispatchCreateManualTaskCommand(
            category: TaskCategory.Farming,
            title: "Pre-title task",
            description: null,
            creationDay: day
        );

        InvokeOnReturnedToTitle(sut);

        Assert.True(dispatcher.HudDisposedBeforeReturnedToTitleDispatch);
        Assert.True(dispatcher.StateStillPopulatedDuringReturnedToTitleDispatch);
        Assert.Null(GetHudViewModel(sut));
        Assert.Equal(0, GetTaskCount(stateStore));
    }

    private static ModEntry CreateEntryWithRuntime(ModRuntime runtime)
    {
        ModEntry entry = new();
        SetRuntime(entry, runtime);
        return entry;
    }

    private static ModRuntime CreateRuntime(RecordingEventDispatcher dispatcher, Store stateStore)
    {
        Mock<IMonitor> monitor = new(MockBehavior.Loose);
        ModLogger logger = new(monitor.Object);
        ModConfig config = new() { EnableDebugMode = false };
        LifecycleCoordinator coordinator = new(logger, dispatcher, stateStore);
        return new ModRuntime(logger, config, dispatcher, coordinator, stateStore);
    }

    private static void SetRuntime(ModEntry entry, ModRuntime runtime)
    {
        FieldInfo runtimeField = Assert.Single(
            typeof(ModEntry).GetFields(BindingFlags.Instance | BindingFlags.NonPublic),
            static field => field.FieldType == typeof(ModRuntime)
        );

        runtimeField.SetValue(entry, runtime);
    }

    private static HudViewModel? GetHudViewModel(ModEntry entry)
    {
        FieldInfo hudViewModelField = Assert.Single(
            typeof(ModEntry).GetFields(BindingFlags.Instance | BindingFlags.NonPublic),
            static field => field.FieldType == typeof(HudViewModel)
        );

        return hudViewModelField.GetValue(entry) as HudViewModel;
    }

    private static void SetHudViewModel(ModEntry entry, HudViewModel? hudViewModel)
    {
        FieldInfo hudViewModelField = Assert.Single(
            typeof(ModEntry).GetFields(BindingFlags.Instance | BindingFlags.NonPublic),
            static field => field.FieldType == typeof(HudViewModel)
        );

        hudViewModelField.SetValue(entry, hudViewModel);
    }

    private static int GetTaskCount(Store stateStore)
    {
        FieldInfo stateContainerField =
            typeof(Store).GetField(
                "_stateContainer",
                BindingFlags.Instance | BindingFlags.NonPublic
            )
            ?? throw new InvalidOperationException(
                "StateStore state container field was not found."
            );

        object stateContainer =
            stateContainerField.GetValue(stateStore)
            ?? throw new InvalidOperationException("StateStore state container was null.");

        MethodInfo getAllMethod =
            stateContainer
                .GetType()
                .GetMethod("GetAll", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("StateContainer.GetAll method was not found.");

        object records =
            getAllMethod.Invoke(stateContainer, Array.Empty<object>())
            ?? throw new InvalidOperationException("StateContainer.GetAll returned null.");

        return ((System.Collections.IEnumerable)records).Cast<object>().Count();
    }

    private static void InvokeOnReturnedToTitle(ModEntry entry)
    {
        MethodInfo method =
            typeof(ModEntry).GetMethod(
                "OnReturnedToTitle",
                BindingFlags.Instance | BindingFlags.NonPublic
            ) ?? throw new InvalidOperationException("OnReturnedToTitle method was not found.");

        method.Invoke(entry, new object?[] { null, null });
    }

    private sealed class RecordingEventDispatcher : IEventDispatcher
    {
        private readonly Store _stateStore;
        private ModEntry? _entry;

        internal RecordingEventDispatcher(Store stateStore)
        {
            _stateStore = stateStore;
        }

        internal bool HudDisposedBeforeReturnedToTitleDispatch { get; private set; }

        internal bool StateStillPopulatedDuringReturnedToTitleDispatch { get; private set; }

        internal void AttachEntry(ModEntry entry)
        {
            _entry = entry;
        }

        public void DispatchGameLaunched() { }

        public void DispatchSaveLoaded() { }

        public void DispatchDayStarted() { }

        public void DispatchReturnedToTitle()
        {
            HudDisposedBeforeReturnedToTitleDispatch =
                _entry is not null && GetHudViewModel(_entry) is null;
            StateStillPopulatedDuringReturnedToTitleDispatch = GetTaskCount(_stateStore) > 0;
        }

        public void DispatchSavingInProgress() { }

        public void DispatchTimeChanged(DayKey currentDay, int currentTime) { }

        public void DispatchUpdateTicked() { }
    }
}
