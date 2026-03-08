// Purpose: Factory for creating TaskIds.

namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>
/// Factory for creating <see cref="TaskId"/> instances from various components.
/// </summary>
internal static class TaskIdFactory
{
    private const string BuiltInPrefix = "BuiltIn";
    private const string TaskBuilderPrefix = "TaskBuilder";

    // Builds TaskIds for built-in generators.
    public static TaskId CreateBuiltIn(
        string generatorId,
        string? subjectIdentifier = null,
        DayKey? dayKey = null
    )
    {
        // Guard Clause. Generator ID is required, other components are optional
        if (string.IsNullOrWhiteSpace(generatorId))
        {
            throw new ArgumentException("Generator ID cannot be null, empty, or whitespace.", nameof(generatorId));
        }

        // Constructs a TaskId in the format: "BuiltIn_{GeneratorId}_{SubjectIdentifier}_{DayKey}"
        return TaskIdFromParts(
            BuiltInPrefix,
            generatorId,
            subjectIdentifier,
            dayKey?.ToString());
    }

    // Builds TaskIds for TaskBuilder-generated tasks.
    public static TaskId CreateTaskBuilder(
        string ruleId,
        string? subjectIdentifier = null,
        DayKey? dayKey = null)
    {
        // Guard Clause. Rule ID is required, other components are optional
        if (string.IsNullOrWhiteSpace(ruleId))
        {
            throw new ArgumentException("Rule ID cannot be null, empty, or whitespace.", nameof(ruleId));
        }

        // Constructs a TaskId in the format: "TaskBuilder_{RuleId}_{SubjectIdentifier}_{DayKey}"
        return TaskIdFromParts(
            TaskBuilderPrefix,
            ruleId,
            subjectIdentifier,
            dayKey?.ToString());
    }

    // Helper method to construct a TaskId from its components. Can take multiple parameters, so if 
    // more components are needed in the future, they can be added to generators above
    private static TaskId TaskIdFromParts(params string?[] parts)
    {
        string taskId = string.Join("_", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        return new TaskId(taskId);
    }
}
