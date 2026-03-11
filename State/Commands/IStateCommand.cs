using JojaAutoTasks.Domain.Identifiers;

namespace JojaAutoTasks.State.Commands;

/// <summary>Represents a state-store command scoped to a single task.</summary>
internal interface IStateCommand
{
    TaskId TaskId { get; }
}
