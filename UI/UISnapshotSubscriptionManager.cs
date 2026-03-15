using JojaAutoTasks.Events;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Ui;

internal static class UiSnapshotSubscriptionManager
{
    private sealed class SnapshotSubscription : IDisposable
    {
        private readonly StateStore _stateStore;
        private readonly EventHandler<SnapshotChangedEventArgs> _handler;

        internal SnapshotSubscription(
            StateStore stateStore,
            EventHandler<SnapshotChangedEventArgs> handler
        )
        {
            _stateStore = stateStore;
            _handler = handler;
        }

        public void Dispose()
        {
            _stateStore.SnapshotChanged -= _handler;
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

        EventHandler<SnapshotChangedEventArgs> handler = (sender, args) =>
        {
            callback(args.CurrentSnapshot);
        };

        _stateStore.SnapshotChanged += handler;
        return new SnapshotSubscription(_stateStore, handler);
    }
}
