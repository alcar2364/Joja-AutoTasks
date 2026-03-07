// Purpose: Defines the immutable TaskID value type used for deterministic task identity.

namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>
/// Represents a canonical, immutable identifier for a task.
/// </summary>
internal readonly struct TaskID : IEquatable<TaskID>
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;
    private readonly string _taskID;
    

    /// <summary>
    /// Initializes a new <see cref="TaskID"/> from a raw identifier string.
    /// </summary>
    /// <param name="taskID">The raw task identifier.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="taskID"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="taskID"/> is empty or whitespace after normalization.
    /// </exception>
    /// 
    public TaskID(string taskID)
    {
        string normalizedTaskID = NormalizeTaskID(taskID);
        ValidateTaskID(normalizedTaskID);
        _taskID = normalizedTaskID;

    }

    // Gets canonical TaskID string
    public string Value => _taskID ?? string.Empty;

    public bool Equals(TaskID other) => Comparer.Equals(_taskID, other._taskID);
    

    public override bool Equals(object? obj) => obj is TaskID other && Equals(other);

    public override int GetHashCode() => Comparer.GetHashCode(_taskID ?? string.Empty);
    public static bool operator ==(TaskID left, TaskID right) => left.Equals(right);
    public static bool operator !=(TaskID left, TaskID right) => !left.Equals(right);
    public override string ToString() => _taskID ?? string.Empty;

    private static string NormalizeTaskID(string? taskID)
    {
        // Normalizes the task ID by trimming whitespace.  
        // TODO: Add addtional normalization logic if needed
        return taskID == null
            ? throw new ArgumentNullException(nameof(taskID), "TaskID cannot be null.")
            : taskID.Trim();
    }

    private static string ValidateTaskID(string? taskID)
    {
        // TODO: Add additional validation if necessary (e.g., check for invalid characters, length constraints, etc.)

        return string.IsNullOrWhiteSpace(taskID)
            ? throw new ArgumentException("TaskID cannot be null or whitespace.", nameof(taskID))
            : taskID;
    }

}

