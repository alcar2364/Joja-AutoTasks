using System.Linq;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.StateStore.Models;


namespace JojaAutoTasks.StateStore.DayBoundary;


internal sealed class ExpirationDetector
{
    internal static IReadOnlyList<TaskId> DetectExpiredTasks(
        StateContainer stateContainer,
        DayKey currentDay)
    {
        int currentSequence = currentDay.ToSequenceNumber();
        var expiredTasks =  stateContainer.GetAll()
            .Where(record => record.CreationDay.ToSequenceNumber() < currentSequence)
            .Select(record => record.Id)
            .ToList();
        
        return expiredTasks;
    }
}
