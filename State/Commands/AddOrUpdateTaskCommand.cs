using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;

namespace JojaAutoTasks.State.Commands;

/// <summary>
/// Represents a command to add a new task or update an existing task in the state store. 
/// </summary>
internal sealed class AddOrUpdateTaskCommand : IStateCommand
{

    public TaskId TaskId { get; }

    internal TaskCategory Category { get; }

    internal TaskSourceType SourceType { get; }

    internal string Title { get; }

    internal string? Description { get; }

    internal int ProgressCurrent { get; }

    internal int ProgressMax { get; }

    internal DayKey CreationDay { get; }
    internal string SourceIdentifier { get; }


    // -- Constructor -- //
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "Command constructor requires explicit parameters for all TaskObject fields to ensure immutability and clarity of command intent.")]

    internal AddOrUpdateTaskCommand(
        TaskId taskId,
        TaskCategory category,
        TaskSourceType sourceType,
        string title,

        string? description,

        int progressCurrent,

        int progressMax,

        DayKey creationDay,
        string sourceIdentifier)
    {
        // -- Guards -- //
        if (taskId == default)
        {
            throw new ArgumentException("TaskId cannot be null or empty.", nameof(taskId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or whitespace.", nameof(title));
        }

        if (description != null && string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be whitespace.", nameof(description));
        }

        if (progressCurrent < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(progressCurrent), "Progress current cannot be negative.");
        }

        if (progressMax <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(progressMax), "Progress max must be greater than zero.");
        }

        if (creationDay == default)
        {
            throw new ArgumentException("CreationDay cannot be the default value.", nameof(creationDay));
        }

        if (string.IsNullOrWhiteSpace(sourceIdentifier))
        {
            throw new ArgumentException("SourceIdentifier cannot be null or whitespace.", nameof(sourceIdentifier));
        }

        TaskId = taskId;
        Category = category;
        SourceType = sourceType;
        Title = title;
        Description = description;
        ProgressCurrent = progressCurrent;
        ProgressMax = progressMax;
        CreationDay = creationDay;
        SourceIdentifier = sourceIdentifier;
    }
}
