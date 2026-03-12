namespace JojaAutoTasks.State.Models;

internal sealed class ManualTaskCounter
{
    private int _nextId;

    internal int IssueNextId()
    {
        return _nextId++;
    }

    internal void Reset()
    {
        _nextId = default;
    }
}
