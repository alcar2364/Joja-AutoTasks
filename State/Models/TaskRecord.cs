using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.State.Models;

internal sealed class TaskRecord
{
    // -- Identity (immutable) -- //
    internal TaskId Id { get; }

    // -- Engine-Owned Fields -- //
    internal TaskCategory Category { get; }
    internal TaskSourceType SourceType { get; }
    internal string Title { get; set; }
    internal string? Description { get; set; }
    internal string SourceIdentifier { get; }
    internal int ProgressCurrent { get; set; }
    internal int ProgressMax { get; }
    internal DayKey CreationDay { get; }

    // -- User-Owned Fields -- //
    internal bool IsPinned { get; set; }

    // -- Command-Specific Fields -- //
    internal TaskStatus Status { get; set; }
    internal DayKey? CompletionDay { get; set; }
    internal DeadlineStoredFields? DeadlineStoredFields { get; set; }



    internal TaskRecord(
        TaskId id,
        TaskCategory category,
        TaskSourceType sourceType,
        string title,
        string? description,
        TaskStatus status,
        int progressCurrent,
        int progressMax,
        DayKey creationDay,
        DayKey? completionDay,
        string sourceIdentifier,
        bool isPinned,
        DeadlineStoredFields? deadlineStoredFields = null)
    {
        Id = id;
        Category = category;
        SourceType = sourceType;
        Title = title;
        Description = description;
        Status = status;
        ProgressCurrent = progressCurrent;
        ProgressMax = progressMax;
        CreationDay = creationDay;
        CompletionDay = completionDay;
        SourceIdentifier = sourceIdentifier;
        IsPinned = isPinned;
        DeadlineStoredFields = deadlineStoredFields;
    }
}
