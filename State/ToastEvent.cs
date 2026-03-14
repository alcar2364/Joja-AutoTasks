using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.State;

internal sealed class ToastEvent
{
    internal ToastType Type { get; }
    internal string TaskTitle { get; }

    public ToastEvent(ToastType type, string taskTitle)
    {
        ArgumentNullException.ThrowIfNull(taskTitle, nameof(taskTitle));
        
        Type = type;
        TaskTitle = taskTitle;
    }

}
