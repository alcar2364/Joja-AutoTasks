using System.Linq;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.State.DayBoundary;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.State;


internal sealed class SnapshotProjector
{

    internal static TaskSnapshot Project(StateContainer stateContainer, DayKey currentDay, int currentTime)
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
                isPinned: record.IsPinned,
                deadlineFields: record.DeadlineStoredFields is null
                    ? null
                    : DeadlineEvaluator.Evaluate(record.DeadlineStoredFields, currentDay, currentTime)))
            .ToList()
            .AsReadOnly();

        return new TaskSnapshot(taskViews, version);
    }
}
