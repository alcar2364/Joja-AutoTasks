namespace JojaAutoTasks.Domain.Tasks;

/// <summary>Identifies which system created a task.</summary>
public enum TaskSourceType
{
    BuiltIn,
    TaskBuilder,
    Manual
}
