using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.State.Models;

internal sealed class DeadlineFields
{
    internal DayKey DueDayKey { get; }
    internal int? ExpiresAtTime { get; }
    internal int DaysRemaining { get; }
    internal bool IsOverdue { get; }
    internal bool IsWindowClosed { get; }

    internal DeadlineFields(
        DayKey dueDayKey,
        int? expiresAtTime,
        int daysRemaining,
        bool isOverdue,
        bool isWindowClosed)
    {
        DueDayKey = dueDayKey;
        ExpiresAtTime = expiresAtTime;
        DaysRemaining = daysRemaining;
        IsOverdue = isOverdue;
        IsWindowClosed = isWindowClosed;
    }

}
