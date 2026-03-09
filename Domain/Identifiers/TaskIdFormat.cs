namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>Formats and validates canonical <see cref="TaskId"/> string shapes.</summary>
internal static class TaskIdFormat
{
    // -- Constants -- //
    public const string BuiltInPrefix = "BuiltIn";
    public const string TaskBuilderPrefix = "TaskBuilder";
    public const string ManualPrefix = "Manual";

    private const char Separator = '_';

    // -- Public API -- //
    internal static string Format(TaskId taskId) => taskId.Value;

    internal static bool TryParse(string? rawId, out TaskId result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(rawId))
        {
            return false;
        }

        string[] parts = rawId.Split(Separator);
        if (parts.Length < 2)
        {
            return false;
        }

        string prefix = parts[0];
        bool shapeIsValid = prefix switch
        {
            BuiltInPrefix => IsValidBuiltInShape(parts),
            TaskBuilderPrefix => IsValidBuiltInShape(parts),
            ManualPrefix => IsValidManualShape(parts),
            _ => false
        };

        if (!shapeIsValid)
        {
            return false;
        }

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

    // -- Private Helpers -- //
    private static bool IsValidBuiltInShape(string[] parts)
    {
        // Built-in and task-builder IDs require a prefix plus generator/rule segment; later segments stay verbatim.
        if (parts.Length < 2)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(parts[1]))
        {
            return false;
        }

        for (int i = 2; i < parts.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parts[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidManualShape(string[] parts)
    {
        // Manual IDs stay two segments so the numeric counter cannot be ambiguous during parsing.
        if (parts.Length != 2)
        {
            return false;
        }

        return int.TryParse(parts[1], out int counter) && counter >= 0;
    }
}
