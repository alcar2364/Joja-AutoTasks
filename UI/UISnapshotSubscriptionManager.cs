using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Ui;

internal static class UiSnapshotSubscriptionManager
{
    private sealed class SnapshotSubscription : IDisposable
    {
        private readonly StateStore _stateStore;
        private readonly Action<TaskSnapshot> _callback;

        internal SnapshotSubscription(StateStore stateStore, Action<TaskSnapshot> callback)
        {
            _stateStore = stateStore;
            _callback = callback;
        }

        public void Dispose()
        {
            _stateStore.SnapshotChanged -= _callback;
        }

    }

    private sealed class NoOpSubscription : IDisposable
    {
        public void Dispose()
        {
            // No-op
        }
    }

    private static StateStore? _stateStore;

    internal static void Initialize(StateStore stateStore)
    {
        _stateStore = stateStore;
    }

    // No lock required, SMAPI mods are single-threaded
    public static IDisposable Subscribe(Action<TaskSnapshot> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        if (_stateStore == null)
        {
            return new NoOpSubscription();
        }

        _stateStore.SnapshotChanged += callback;
        return new SnapshotSubscription(_stateStore, callback);
    }
}