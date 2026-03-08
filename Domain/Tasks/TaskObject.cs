// Purpose: Defines an object representing a task in the domain model. This is the core data 
// structure that will be used throughout the system to represent tasks, their properties, 
// and their state. It is immutable.
using JojaAutoTasks.Domain.Identifiers;
namespace JojaAutoTasks.Domain.Tasks;
/// <summary>
/// Represents a task in the domain model.
/// </summary>
internal sealed class TaskObject
{
    public TaskId Id { get; }

    //TODO: Do we need Task Type here? Or can we infer it from TaskId?

    public TaskCategory Category { get; }
    public TaskSourceType SourceType { get; }
    public string Title { get; }
    public string? Description { get; }
    public TaskStatus Status { get; }
    public int ProgressCurrent { get; }
    public int ProgressMax { get; }
    public DayKey CreationDay { get; }
    public DayKey? CompletionDay { get; }
    public string SourceIdentifier { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Domain model constructor requires explicit parameters for immutable TaskObject fields.")]

    public TaskObject(
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
        string sourceIdentifier
    )
    {
        // Guard clauses for required fields
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException(
            "Title cannot be null or whitespace.", nameof(title));

        if (string.IsNullOrWhiteSpace(sourceIdentifier)) throw new ArgumentException(
            "Source identifier cannot be null or whitespace.", nameof(sourceIdentifier));

        if (progressCurrent < 0) throw new ArgumentOutOfRangeException(
            nameof(progressCurrent), "Progress current cannot be negative.");
        if (progressMax <= 0) throw new ArgumentOutOfRangeException(
            nameof(progressMax), "Progress max must be greater than zero.");

        if (status == TaskStatus.Completed && !completionDay.HasValue) throw new ArgumentException(
            "Completion day must be provided for completed tasks.", nameof(completionDay));

        if (status == TaskStatus.Completed && progressCurrent < progressMax) throw new ArgumentException(
            "Current progress must be greater than or equal to progress max for completed tasks.",
            nameof(progressCurrent));

        if (status == TaskStatus.Incomplete && completionDay.HasValue) throw new ArgumentException(
            "Completion day should not be provided for non-completed tasks.", nameof(completionDay));

        // Property assignments
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
    }
}
