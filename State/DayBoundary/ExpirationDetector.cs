using System.Linq;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.State.Models;


namespace JojaAutoTasks.State.DayBoundary;


internal sealed class ExpirationDetector
{
    internal static IReadOnlyList<TaskId> DetectExpiredIds(
        StateContainer stateContainer,
        DayKey currentDay)
    {
        int currentSequence = currentDay.ToSequenceNumber();
        var expiredIds = stateContainer.GetAll()
            .Where(record => record.CreationDay.ToSequenceNumber() < currentSequence)
            .Select(record => record.Id)
            .ToList();

        return expiredIds;
    }
}
