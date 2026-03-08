// Purpose: Formats TaskId values for deterministic construction and parsing, ensuring consistent identity 
// generation across the system.

namespace JojaAutoTasks.Domain.Identifiers;

internal static class TaskIdFormat
{
    // -- Constants --

    public const string BuiltInPrefix = "BuiltIn";
    public const string TaskBuilderPrefix = "TaskBuilder";
    public const string ManualPrefix = "Manual";
    private const char Separator = '_';

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
            TaskBuilderPrefix => IsValidBuiltInShape(parts),
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
        // Optional suffix components may contain underscores.

        if (parts.Length < 2)
            return false;

        if (string.IsNullOrWhiteSpace(parts[1]))
            return false;

        for (int i = 2; i < parts.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parts[i]))
                return false;
        }

        return true;
    }

    private static bool IsValidManualShape(string[] parts)
    {
        // Must have exactly: [Manual, Counter]

        if (parts.Length != 2)
            return false;

        return int.TryParse(parts[1], out int counter) && counter >= 0;
    }
}
