namespace JojaAutoTasks.State.Models;

internal sealed class ManualTaskCounter
{
    private int _nextId = 1;

    internal int IssueNextId()
    {
        return _nextId++;
    }
}
