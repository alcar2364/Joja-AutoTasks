using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.StateStore.Models;

internal sealed class TaskRecord
{
    internal TaskId Id { get; }
    internal TaskCategory Category { get; }
    internal TaskSourceType SourceType { get; }
    internal string Title { get; set; }
    internal string? Description { get; set; }
    internal TaskStatus Status { get; set; }
    internal int ProgressCurrent { get; set; }
    internal int ProgressMax { get; set; }
    internal DayKey CreationDay { get; }
    internal DayKey? CompletionDay { get; set; }
    internal string SourceIdentifier { get; }
    internal bool IsPinned { get; set; }

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
