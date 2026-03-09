namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>Builds canonical <see cref="DayKey"/> values from explicit components.</summary>
internal static class DayKeyFactory
{
    // -- Public API -- //
    public static DayKey Create(int year, string seasonToken, int day)
    {
        // -- Guards -- //
        if (string.IsNullOrWhiteSpace(seasonToken))
        {
            throw new ArgumentException(
                "Season token cannot be null, empty, or whitespace.",
                nameof(seasonToken));
        }

        string rawKey = $"Year{year}-{seasonToken}{day}";
        return new DayKey(rawKey);
    }
}
