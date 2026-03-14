using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.State.DayBoundary;

internal static class DeadlineEvaluator
{
    internal static DeadlineFields Evaluate(
        DeadlineStoredFields stored,
        DayKey currentDay,
        int currentTime)
    {
        int dueDay = stored.DueDayKey.ToSequenceNumber();
        int currentDaySequence = currentDay.ToSequenceNumber();
        int? expiresAtTime = stored.ExpiresAtTime;

        int daysRemaining = dueDay - currentDaySequence;
        bool isOverdue = currentDaySequence > dueDay;
        bool isWindowClosed =
            currentDaySequence == dueDay
            && expiresAtTime.HasValue
            && currentTime >= expiresAtTime.Value;

        return new DeadlineFields(
            dueDayKey: stored.DueDayKey,
            expiresAtTime: stored.ExpiresAtTime,
            daysRemaining,
            isOverdue,
            isWindowClosed
        );
    }
}
