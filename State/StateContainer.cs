using System.Diagnostics.CodeAnalysis;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.State;

internal sealed class StateContainer
{
    // -- State Fields -- //

    private readonly Dictionary<TaskId, TaskRecord> _tasksMap = new();
    private long _version;
    internal long Version => _version;

    // -- State Accessors -- //

    internal bool TryGet(TaskId id, [NotNullWhen(true)] out TaskRecord? record)
    => _tasksMap.TryGetValue(id, out record);

    internal void Set(TaskId id, TaskRecord newRecord)
    {
        _tasksMap[id] = newRecord;
        IncrementVersion();
    }

    internal void Remove(TaskId id)
    {
        if (_tasksMap.Remove(id))
        {
            IncrementVersion();
        }
    }

    internal IReadOnlyCollection<TaskRecord> GetAll() => _tasksMap.Values;

    internal void Clear()
    {
        _tasksMap.Clear();
        _version = default;
    }

    // -- Private Helpers -- //
    private void IncrementVersion() => _version++;
}
