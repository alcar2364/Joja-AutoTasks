using System.Reflection;
using JojaAutoTasks.Events;
using Xunit;

namespace JojaAutoTasks.Tests.Events;

public class EventDispatcherTests
{
    [Fact]
    public void DispatchMethods_WhenInvokedInCanonicalOrder_DoNotThrow()
    {
        EventDispatcher sut = new EventDispatcher();

        Exception? exception = Record.Exception(() =>
        {
            sut.DispatchGameLaunched();
            sut.DispatchSaveLoaded();
            sut.DispatchDayStarted();
            sut.DispatchReturnedToTitle();
            sut.DispatchSavingInProgress();
            sut.DispatchUpdateTicked();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void DispatchMethods_WhenInvokedInDifferentOrders_KeepDispatcherStateUnchanged()
    {
        EventDispatcher canonical = new EventDispatcher();
        EventDispatcher reversed = new EventDispatcher();

        object?[] beforeCanonical = CaptureInstanceFieldValues(canonical);
        object?[] beforeReversed = CaptureInstanceFieldValues(reversed);

        canonical.DispatchGameLaunched();
        canonical.DispatchSaveLoaded();
        canonical.DispatchDayStarted();
        canonical.DispatchReturnedToTitle();
        canonical.DispatchSavingInProgress();
        canonical.DispatchUpdateTicked();

        reversed.DispatchUpdateTicked();
        reversed.DispatchSavingInProgress();
        reversed.DispatchReturnedToTitle();
        reversed.DispatchDayStarted();
        reversed.DispatchSaveLoaded();
        reversed.DispatchGameLaunched();

        object?[] afterCanonical = CaptureInstanceFieldValues(canonical);
        object?[] afterReversed = CaptureInstanceFieldValues(reversed);

        Assert.Equal(beforeCanonical, afterCanonical);
        Assert.Equal(beforeReversed, afterReversed);
        Assert.Equal(afterCanonical, afterReversed);
    }

    [Fact]
    public void DispatcherType_HasNoInstanceFields_ForNoOpDeterministicProcessing()
    {
        FieldInfo[] fields = typeof(EventDispatcher).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        Assert.Empty(fields);
    }

    [Theory]
    [InlineData(nameof(IEventDispatcher.DispatchGameLaunched))]
    [InlineData(nameof(IEventDispatcher.DispatchSaveLoaded))]
    [InlineData(nameof(IEventDispatcher.DispatchDayStarted))]
    [InlineData(nameof(IEventDispatcher.DispatchReturnedToTitle))]
    [InlineData(nameof(IEventDispatcher.DispatchSavingInProgress))]
    [InlineData(nameof(IEventDispatcher.DispatchUpdateTicked))]
    public void DispatchMethods_HaveNoOperationalIlBeyondNopAndReturn(string methodName)
    {
        MethodInfo method = typeof(EventDispatcher).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Method '{methodName}' was not found.");

        byte[] il = method.GetMethodBody()?.GetILAsByteArray()
            ?? throw new InvalidOperationException($"Method body for '{methodName}' was not found.");

        Assert.NotEmpty(il);

        foreach (byte opcode in il)
        {
            // No-op contract: allow only NOP (0x00) and RET (0x2A).
            Assert.True(opcode is 0x00 or 0x2A, $"Method '{methodName}' contains non-no-op opcode 0x{opcode:X2}.");
        }
    }

    private static object?[] CaptureInstanceFieldValues(EventDispatcher dispatcher)
    {
        FieldInfo[] fields = typeof(EventDispatcher).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        object?[] values = new object?[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            values[i] = fields[i].GetValue(dispatcher);
        }

        return values;
    }
}
