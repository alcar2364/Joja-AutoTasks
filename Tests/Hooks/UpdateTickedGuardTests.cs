using System.Reflection;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.Startup;

namespace JojaAutoTasks.Tests.Hooks;

/// <summary>Tests throttled update-tick forwarding through <see cref="ModEntry" />.</summary>
public class UpdateTickedGuardTests
{
    // -- Public API -- //
    [Fact]
    public void ShouldForwardUpdateTick_ThrottlesSignalsToConfiguredInterval()
    {
        ModEntry sut = CreateEntryWithRuntime(out _);

        bool firstTick = InvokeShouldForwardUpdateTick(sut, 0);
        bool beforeBoundary = InvokeShouldForwardUpdateTick(sut, 359);
        bool atBoundary = InvokeShouldForwardUpdateTick(sut, 360);
        bool afterBoundary = InvokeShouldForwardUpdateTick(sut, 361);

        Assert.True(firstTick);
        Assert.False(beforeBoundary);
        Assert.True(atBoundary);
        Assert.False(afterBoundary);
    }

    [Fact]
    public void ForwardUpdateTickIfDue_DoesNotForwardWhileInsideThrottleWindow()
    {
        ModEntry sut = CreateEntryWithRuntime(out RecordingEventDispatcher dispatcher);

        sut.ForwardUpdateTickIfDue(0);
        sut.ForwardUpdateTickIfDue(0);

        Assert.Equal(1, dispatcher.UpdateTickedDispatchCount);
        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchUpdateTicked), dispatcher.Calls[0]);
    }

    [Fact]
    public void ForwardUpdateTickIfDue_GuardBlockPathDoesNotRequireRuntimeAccess()
    {
        ModEntry sut = CreateEntryWithRuntime(out RecordingEventDispatcher dispatcher);

        sut.ForwardUpdateTickIfDue(0);
        SetRuntime(sut, null);

        Exception? exception = Record.Exception(() => sut.ForwardUpdateTickIfDue(0));

        Assert.Null(exception);
        Assert.Equal(1, dispatcher.UpdateTickedDispatchCount);
    }

    // -- Private Helpers -- //
    private static ModEntry CreateEntryWithRuntime(out RecordingEventDispatcher dispatcher)
    {
        ModLogger logger = new(null);
        ModConfig config = new() { EnableDebugMode = false };
        dispatcher = new RecordingEventDispatcher();
        LifecycleCoordinator coordinator = new(logger, dispatcher);
        ModRuntime runtime = new(logger, config, dispatcher, coordinator);

        ModEntry entry = new();
        SetRuntime(entry, runtime);
        return entry;
    }

    private static void SetRuntime(ModEntry entry, ModRuntime? runtime)
    {
        FieldInfo[] runtimeFields = typeof(ModEntry)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(static field => field.FieldType == typeof(ModRuntime))
            .ToArray();

        if (runtimeFields.Length == 0)
        {
            throw new InvalidOperationException("ModEntry runtime field was not found.");
        }

        if (runtimeFields.Length > 1)
        {
            throw new InvalidOperationException("ModEntry runtime field was ambiguous.");
        }

        FieldInfo runtimeField = runtimeFields[0];
        runtimeField.SetValue(entry, runtime);
    }

    private static bool InvokeShouldForwardUpdateTick(ModEntry entry, uint tick)
    {
        MethodInfo method = typeof(ModEntry).GetMethod("ShouldForwardUpdateTick", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ShouldForwardUpdateTick method was not found.");

        return (bool)(method.Invoke(entry, new object[] { tick })
            ?? throw new InvalidOperationException("ShouldForwardUpdateTick returned null."));
    }

    private sealed class RecordingEventDispatcher : IEventDispatcher
    {
        internal int UpdateTickedDispatchCount { get; private set; }

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
            UpdateTickedDispatchCount++;
            Calls.Add(nameof(DispatchUpdateTicked));
        }
    }
}
