using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Models;
using JojaAutoTasks.Domain.Tasks;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.State.Handlers;

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

        if (command.SourceType == TaskSourceType.Manual)
        {
            // User-authored edits preserve engine-owned progress/status fields.
            existingRecord.Title = command.Title;
            existingRecord.Description = command.Description;

            state.Set(command.TaskId, existingRecord);
            return;
        }

        // Engine-authored updates preserve user-owned fields such as pin and completion state.
        TaskRecord updatedRecord = new(
            id: existingRecord.Id,
            category: command.Category,
            sourceType: command.SourceType,
            title: command.Title,
            description: command.Description,
            status: existingRecord.Status,
            progressCurrent: command.ProgressCurrent,
            progressMax: command.ProgressMax,
            creationDay: command.CreationDay,
            completionDay: existingRecord.CompletionDay,
            sourceIdentifier: command.SourceIdentifier,
            isPinned: existingRecord.IsPinned);

        state.Set(command.TaskId, updatedRecord);
    }
}
