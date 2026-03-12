using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.UI;

internal static class UISnapshotSubscriptionManager
{
    private static StateStore? _stateStore;

    internal static void Initialize(StateStore stateStore)
    {
        _stateStore = stateStore;
    }

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

    private class SnapshotSubscription : IDisposable
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
            // Unsubscribe logic goes here
            throw new NotImplementedException("Unsubscription logic is not yet implemented.");
        }

    }

    private class NoOpSubscription : IDisposable
    {
        public void Dispose()
        {
            // No-op
        }
    }
}