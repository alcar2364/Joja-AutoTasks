using JojaAutoTasks.State;

namespace JojaAutoTasks.Ui;

internal static class UiToastSubscriptionManager
{
    private sealed class ToastSubscription : IDisposable
    {
        private readonly StateStore _stateStore;
        private readonly Action<ToastEvent> _callback;

        internal ToastSubscription(StateStore stateStore, Action<ToastEvent> callback)
        {
            _stateStore = stateStore;
            _callback = callback;
        }

        public void Dispose()
        {
            _stateStore.ToastRequested -= _callback;
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
    public static IDisposable Subscribe(Action<ToastEvent> callback)
    {
        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback));
        }

        if (_stateStore == null)
        {
            return new NoOpSubscription();
        }

        _stateStore.ToastRequested += callback;
        return new ToastSubscription(_stateStore, callback);
    }
}
