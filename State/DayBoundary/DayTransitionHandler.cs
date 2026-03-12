using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Handlers;

namespace JojaAutoTasks.State.DayBoundary;

internal sealed class DayTransitionHandler
{
    internal static void RemoveExpiredTasks(
        IReadOnlyList<TaskId> expiredIds,
        StateContainer stateContainer,
        RemoveTaskCommandHandler removeTaskHandler)
    {
        foreach (var expiredId in expiredIds)
        {
            removeTaskHandler.Handle(new RemoveTaskCommand(expiredId), stateContainer);
        }
    }
}
