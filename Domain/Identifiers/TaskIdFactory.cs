namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>Builds <see cref="TaskId"/> values from generator-specific components.</summary>
internal static class TaskIdFactory
{
    // -- Constants -- //
    private const string BuiltInPrefix = "BuiltIn";
    private const string TaskBuilderPrefix = "TaskBuilder";

    // -- Public API -- //
    public static TaskId CreateBuiltIn(
        string generatorId,
        string? subjectIdentifier = null,
        DayKey? dayKey = null)
    {
        // -- Guards -- //
        if (string.IsNullOrWhiteSpace(generatorId))
        {
            throw new ArgumentException("Generator ID cannot be null, empty, or whitespace.", nameof(generatorId));
        }

        return TaskIdFromParts(
            BuiltInPrefix,
            generatorId,
            subjectIdentifier,
            dayKey?.ToString());
    }

    public static TaskId CreateTaskBuilder(
        string ruleId,
        string? subjectIdentifier = null,
        DayKey? dayKey = null)
    {
        // -- Guards -- //
        if (string.IsNullOrWhiteSpace(ruleId))
        {
            throw new ArgumentException("Rule ID cannot be null, empty, or whitespace.", nameof(ruleId));
        }

        return TaskIdFromParts(
            TaskBuilderPrefix,
            ruleId,
            subjectIdentifier,
            dayKey?.ToString());
    }

    // -- Private Helpers -- //
    private static TaskId TaskIdFromParts(params string?[] parts)
    {
        string taskId = string.Join("_", parts.Where(static part => !string.IsNullOrWhiteSpace(part)));
        return new TaskId(taskId);
    }
}
