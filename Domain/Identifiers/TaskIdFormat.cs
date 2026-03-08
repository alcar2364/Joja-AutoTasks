// Purpose: Formats TaskId values for deterministic construction and parsing, ensuring consistent identity 
// generation across the system.

namespace JojaAutoTasks.Domain.Identifiers;

internal static class TaskIdFormat
{
    // -- Constants --

    public const string BuiltInPrefix = "BuiltIn";
    public const string TaskBuilderPrefix = "TaskBuilder";
    private const string ManualPrefix = "Manual";
    private const char Separator = ':';

    // -- Public API --
    internal static string Format(TaskId taskId) => taskId.Value;

    internal static bool TryParse(string? rawId, out TaskId result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(rawId))
            return false;

        string[] parts = rawId.Split(Separator);

        if (parts.Length < 2)
            return false;

        string prefix = parts[0];

        bool shapeIsValid = prefix switch
        {
            BuiltInPrefix => IsValidBuiltInShape(parts),
            TaskBuilderPrefix => IsValidTaskBuilderShape(parts),
            ManualPrefix => IsValidManualShape(parts),
            _ => false
        };

        if (!shapeIsValid)
            return false;

        try
        {
            result = new TaskId(rawId);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    // -- Private Helpers --

    private static bool IsValidBuiltInShape(string[] parts)
    {
        // Must have at least: [BuiltIn, GeneratorId]
        // May have at most: [BuiltIn, GeneratorId, SubjectIdentifier, DayKey]

        if (parts.Length < 2 || parts.Length > 4)
            return false;

        return !string.IsNullOrWhiteSpace(parts[1]);
    }

    // IsValidTaskBuilderShape has the same shape requirements as IsValidBuiltInShape, just with a 
    // different prefix. Building separate methods for clarity and future-proofing in case TaskBuilder
    // shapes diverge from BuiltIn shapes later on.

    private static bool IsValidTaskBuilderShape(string[] parts)
    {
        // Must have at least: [RuleId, GeneratorId]
        // May have at most: [RuleId, GeneratorId, SubjectIdentifier, DayKey]

        if (parts.Length < 2 || parts.Length > 4)
            return false;

        return !string.IsNullOrWhiteSpace(parts[1]);
    }

    private static bool IsValidManualShape(string[] parts)
    {
        // Must have exactly: [Manual, Counter]

        if (parts.Length != 2)
            return false;

        return int.TryParse(parts[1], out int counter) && counter >= 0;
    }
}
