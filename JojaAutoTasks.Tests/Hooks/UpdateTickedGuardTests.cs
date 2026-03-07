using System.Reflection;
using JojaAutoTasks.Configuration;
using JojaAutoTasks.Events;
using JojaAutoTasks.Infrastructure.Logging;
using JojaAutoTasks.Lifecycle;
using JojaAutoTasks.Startup;
using Moq;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace JojaAutoTasks.Tests.Hooks;

public class UpdateTickedGuardTests
{
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
    public void OnUpdateTicked_DoesNotForwardWhileInsideThrottleWindow()
    {
        ModEntry sut = CreateEntryWithRuntime(out RecordingEventDispatcher dispatcher);
        UpdateTickedEventArgs tickArgs = CreateUpdateTickedEventArgs();

        InvokeOnUpdateTicked(sut, tickArgs);
        InvokeOnUpdateTicked(sut, tickArgs);

        Assert.Equal(1, dispatcher.UpdateTickedDispatchCount);
        Assert.Single(dispatcher.Calls);
        Assert.Equal(nameof(IEventDispatcher.DispatchUpdateTicked), dispatcher.Calls[0]);
    }

    [Fact]
    public void OnUpdateTicked_GuardBlockPathDoesNotRequireRuntimeAccess()
    {
        ModEntry sut = CreateEntryWithRuntime(out RecordingEventDispatcher dispatcher);
        UpdateTickedEventArgs tickArgs = CreateUpdateTickedEventArgs();

        InvokeOnUpdateTicked(sut, tickArgs);
        SetRuntime(sut, null);

        Exception? exception = Record.Exception(() => InvokeOnUpdateTicked(sut, tickArgs));

        Assert.Null(exception);
        Assert.Equal(1, dispatcher.UpdateTickedDispatchCount);
    }

    private static ModEntry CreateEntryWithRuntime(out RecordingEventDispatcher dispatcher)
    {
        Mock<IMonitor> monitor = new(MockBehavior.Loose);
        ModLogger logger = new(monitor.Object);
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
        FieldInfo runtimeField = typeof(ModEntry).GetField("runtime", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ModEntry runtime field was not found.");

        runtimeField.SetValue(entry, runtime);
    }

    private static bool InvokeShouldForwardUpdateTick(ModEntry entry, uint tick)
    {
        MethodInfo method = typeof(ModEntry).GetMethod("ShouldForwardUpdateTick", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("ShouldForwardUpdateTick method was not found.");

        return (bool)(method.Invoke(entry, new object[] { tick })
            ?? throw new InvalidOperationException("ShouldForwardUpdateTick returned null."));
    }

    private static void InvokeOnUpdateTicked(ModEntry entry, UpdateTickedEventArgs tickArgs)
    {
        MethodInfo method = typeof(ModEntry).GetMethod("OnUpdateTicked", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("OnUpdateTicked method was not found.");

        method.Invoke(entry, new object?[] { null, tickArgs });
    }

    private static UpdateTickedEventArgs CreateUpdateTickedEventArgs()
    {
        ConstructorInfo[] constructors = typeof(UpdateTickedEventArgs)
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (ConstructorInfo constructor in constructors)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            object?[] baseline = parameters.Select(static parameter => CreateDefault(parameter.ParameterType)).ToArray();

            if (TryConstructWithCandidate(constructor, baseline, out UpdateTickedEventArgs? args) && args is not null)
            {
                return args;
            }
        }

        throw new InvalidOperationException("Unable to construct UpdateTickedEventArgs for test execution.");
    }

    private static bool TryConstructWithCandidate(
        ConstructorInfo constructor,
        object?[] candidate,
        out UpdateTickedEventArgs? args)
    {
        try
        {
            args = (UpdateTickedEventArgs)constructor.Invoke(candidate);
            return true;
        }
        catch (TargetInvocationException)
        {
            args = null;
            return false;
        }
        catch (ArgumentException)
        {
            args = null;
            return false;
        }
    }

    private static object? CreateDefault(Type type)
    {
        Type normalizedType = Nullable.GetUnderlyingType(type) ?? type;

        if (!normalizedType.IsValueType)
        {
            return null;
        }

        return Activator.CreateInstance(normalizedType);
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
