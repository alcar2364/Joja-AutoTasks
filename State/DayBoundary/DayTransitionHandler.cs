using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.State.DayBoundary;

internal sealed class DayTransitionHandler
{
    internal static void RemoveExpiredTasks(IReadOnlyList<TaskId> expiredIds, StateContainer stateContainer)
    {
        foreach (var expiredId in expiredIds)
        {
            stateContainer.Remove(expiredId);
        }
    }
}
