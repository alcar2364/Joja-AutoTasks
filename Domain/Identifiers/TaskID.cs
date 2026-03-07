// Purpose: Defines the immutable TaskID value type used for deterministic task identity.

namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>
/// Represents a canonical, immutable identifier for a task.
/// </summary>
internal readonly struct TaskId : IEquatable<TaskId>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _taskId;
    

    /// <summary>
    /// Initializes a new <see cref="TaskId"/> from a raw identifier string.
    /// </summary>
    /// <param name="taskId">The raw task identifier.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="taskId"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="taskId"/> is empty or whitespace after normalization.
    /// </exception>
    public TaskId(string taskId)
    {
        string normalizedTaskId = IdentifierUtility.NormalizeIdentifier(taskId);
        IdentifierUtility.ValidateIdentifier(normalizedTaskId);
        _taskId = normalizedTaskId;

    }

    public string Value => _taskId ?? string.Empty;

    public bool Equals(TaskId other) => Comparer.Equals(_taskId, other._taskId);
    public override bool Equals(object? obj) => obj is TaskId other && Equals(other);
    public override int GetHashCode() => Comparer.GetHashCode(_taskId ?? string.Empty);
    public static bool operator ==(TaskId left, TaskId right) => left.Equals(right);
    public static bool operator !=(TaskId left, TaskId right) => !left.Equals(right);
    public override string ToString() => _taskId ?? string.Empty;

}

