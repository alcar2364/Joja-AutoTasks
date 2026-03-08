// Purpose: Factory for constructing canonical DayKey instances from component parts.


namespace JojaAutoTasks.Domain.Identifiers;

/// <summary>
/// Factory for constructing <see cref="DayKey"/> instances from explicit year, season, and day
/// components.
/// </summary>
internal static class DayKeyFactory
{
    public static DayKey Create(int year, string seasonToken, int day)
    {
        // Guard Clause
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
