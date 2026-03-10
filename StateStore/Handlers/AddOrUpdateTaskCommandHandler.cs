using JojaAutoTasks.StateStore.Commands;
using JojaAutoTasks.StateStore.Models;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.StateStore.Handlers;

internal sealed class AddOrUpdateTaskCommandHandler : ICommandHandler<AddOrUpdateTaskCommand>
{
    public void Handle(AddOrUpdateTaskCommand command, StateContainer state)
    {
        if (!state.TryGet(command.TaskId, out TaskRecord? existingRecord))
        {
            TaskRecord newRecord = new(
                id: command.TaskId,
                category: command.Category,
                sourceType: command.SourceType,
                title: command.Title,
                description: command.Description,
                sourceIdentifier: command.SourceIdentifier,
                progressCurrent: command.ProgressCurrent,
                progressMax: command.ProgressMax,
                creationDay: command.CreationDay,
                isPinned: false,
                status: TaskStatus.Incomplete,
                completionDay: null);

            state.Set(command.TaskId, newRecord);
            return;
        }

        if (existingRecord.SourceType == TaskSourceType.Manual)
        {
            existingRecord.Title = command.Title;
            existingRecord.Description = command.Description;
            existingRecord.ProgressCurrent = command.ProgressCurrent;

            state.Set(command.TaskId, existingRecord);
        }
    }
}
