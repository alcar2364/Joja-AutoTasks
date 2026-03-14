using System.Reflection;
using JojaAutoTasks.State.Models;
using JojaAutoTasks.Ui;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.Ui;

public sealed class HudHostLifecycleTests
{
    [Fact]
    public void LifecycleTransitions_LeaveExactlyOneActiveSnapshotSubscriptionAfterDayStartedRecreation()
    {
        Store stateStore = CreateInitializedStore();
        ModEntryHudLifecycle lifecycle = CreateLifecycleHarness();

        lifecycle.HandleSaveLoaded();
        lifecycle.HandleDayStarted();
        lifecycle.HandleDayStarted();

        int hostSnapshotSubscriptionCount = CountStateStoreSubscribers(
            stateStore,
            "SnapshotChanged",
            IsHudHostSnapshotHandler);

        Assert.Equal(1, hostSnapshotSubscriptionCount);
    }

    [Fact]
    public void LifecycleTransitions_LeaveExactlyOneActiveToastSubscriptionAfterDayStartedRecreation()
    {
        Store stateStore = CreateInitializedStore();
        ModEntryHudLifecycle lifecycle = CreateLifecycleHarness();

        lifecycle.HandleSaveLoaded();
        lifecycle.HandleDayStarted();
        lifecycle.HandleDayStarted();

        int hostToastSubscriptionCount = CountStateStoreSubscribers(
            stateStore,
            "ToastRequested",
            IsHudHostToastHandler);

        Assert.Equal(1, hostToastSubscriptionCount);
    }

    [Fact]
    public void LifecycleTransitions_DisposePriorHostBeforeReplacementSubscription()
    {
        Store stateStore = CreateInitializedStore();
        int factoryCallCount = 0;

        ModEntryHudLifecycle lifecycle = CreateLifecycleHarness(
            createHost: viewModel =>
            {
                factoryCallCount++;

                int activeSnapshotHandlers = CountStateStoreSubscribers(
                    stateStore,
                    "SnapshotChanged",
                    IsHudHostSnapshotHandler);

                Assert.Equal(0, activeSnapshotHandlers);
                return new HudHost(viewModel);
            });

        lifecycle.HandleSaveLoaded();
        lifecycle.HandleDayStarted();

        Assert.Equal(2, factoryCallCount);
    }

    [Fact]
    public void LifecycleTransitions_UseFreshHudViewModelOnDayStartedRecreation()
    {
        _ = CreateInitializedStore();
        ModEntryHudLifecycle lifecycle = CreateLifecycleHarness();

        lifecycle.HandleSaveLoaded();
        HudViewModel firstViewModel = Assert.IsType<HudViewModel>(lifecycle.CurrentViewModel);

        lifecycle.HandleDayStarted();
        HudViewModel secondViewModel = Assert.IsType<HudViewModel>(lifecycle.CurrentViewModel);

        Assert.NotSame(firstViewModel, secondViewModel);

        int oldViewModelHostHandlerCount = CountHudViewModelNotificationSubscribers(
            firstViewModel,
            IsHudHostNotificationHandler);

        int replacementViewModelHostHandlerCount = CountHudViewModelNotificationSubscribers(
            secondViewModel,
            IsHudHostNotificationHandler);

        Assert.Equal(0, oldViewModelHostHandlerCount);
        Assert.Equal(1, replacementViewModelHostHandlerCount);
    }

    private static ModEntryHudLifecycle CreateLifecycleHarness(
        Func<HudViewModel>? createViewModel = null,
        Func<HudViewModel, HudHost>? createHost = null)
    {
        return new ModEntryHudLifecycle(
            createViewModel ?? (() => new HudViewModel()),
            createHost ?? (viewModel => new HudHost(viewModel)));
    }

    private static Store CreateInitializedStore()
    {
        Store stateStore = new();
        UiSnapshotSubscriptionManager.Initialize(stateStore);
        UiToastSubscriptionManager.Initialize(stateStore);
        return stateStore;
    }

    private static int CountStateStoreSubscribers(
        Store stateStore,
        string eventFieldName,
        Func<Delegate, bool> predicate)
    {
        FieldInfo? field = typeof(Store).GetField(eventFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);

        Delegate? handlers = field!.GetValue(stateStore) as Delegate;
        if (handlers is null)
        {
            return 0;
        }

        return handlers
            .GetInvocationList()
            .Where(predicate)
            .Count();
    }

    private static int CountHudViewModelNotificationSubscribers(
        HudViewModel viewModel,
        Func<Delegate, bool> predicate)
    {
        FieldInfo? field = typeof(HudViewModel).GetField(
            "NotificationRequested",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(field);

        Delegate? handlers = field!.GetValue(viewModel) as Delegate;
        if (handlers is null)
        {
            return 0;
        }

        return handlers
            .GetInvocationList()
            .Where(predicate)
            .Count();
    }

    private static bool IsHudHostSnapshotHandler(Delegate handler)
    {
        return handler.Method.DeclaringType == typeof(HudHost)
               && handler.Method.Name == "OnSnapshotReceived";
    }

    private static bool IsHudHostToastHandler(Delegate handler)
    {
        return handler.Method.DeclaringType == typeof(HudHost)
               && handler.Method.Name == "OnToastReceived";
    }

    private static bool IsHudHostNotificationHandler(Delegate handler)
    {
        return handler.Method.DeclaringType == typeof(HudHost)
               && handler.Method.Name == "OnNotificationRequested";
    }
}