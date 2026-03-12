using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.State.Models;

internal sealed record TaskView
{
    // -- Identity (immutable) -- //
    internal TaskId Id { get; }

    // -- Engine-Owned Fields -- //
    internal TaskCategory Category { get; }
    internal TaskSourceType SourceType { get; }
    internal string Title { get; }
    internal string? Description { get; }
    internal string SourceIdentifier { get; }
    internal int ProgressCurrent { get; }
    internal int ProgressMax { get; }
    internal DayKey CreationDay { get; }

    // -- User-Owned Fields -- //
    internal bool IsPinned { get; }

    // -- Command-Specific Fields -- //
    internal TaskStatus Status { get; }
    internal DayKey? CompletionDay { get; }


    internal TaskView(
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
        bool isPinned)
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
    }
}
