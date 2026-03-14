using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.State.Models;

internal sealed class DeadlineStoredFields
{
    internal DayKey DueDayKey { get; }
    internal int? ExpiresAtTime { get; }

    internal DeadlineStoredFields(DayKey dueDayKey, int? expiresAtTime)
    {
        if (dueDayKey == default)
        {
            throw new ArgumentException("DueDayKey cannot be default.", nameof(dueDayKey));
        }

        DueDayKey = dueDayKey;
        ExpiresAtTime = expiresAtTime;
    }
}
