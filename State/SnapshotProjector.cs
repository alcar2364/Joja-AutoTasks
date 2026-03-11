using System.Linq;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.State;


internal sealed class SnapshotProjector
{

    internal static TaskSnapshot Project(StateContainer stateContainer)
    {
        long version = stateContainer.Version;

        var taskRecords = stateContainer.GetAll();

        var taskViews = taskRecords
            .OrderBy(record => record.Id.Value)
            .Select(record => new TaskView(
                id: record.Id,
                category: record.Category,
                sourceType: record.SourceType,
                title: record.Title,
                description: record.Description,
                status: record.Status,
                progressCurrent: record.ProgressCurrent,
                progressMax: record.ProgressMax,
                creationDay: record.CreationDay,
                completionDay: record.CompletionDay,
                sourceIdentifier: record.SourceIdentifier,
                isPinned: record.IsPinned))
            .ToList();

        return new TaskSnapshot(taskViews, version);
    }
}
