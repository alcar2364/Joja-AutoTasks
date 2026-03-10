using StardewModdingAPI;

namespace JojaAutoTasks.Infrastructure.Logging;

/// <summary>Writes event-tagged messages through the SMAPI monitor.</summary>
public class ModLogger
{
    // -- Dependencies -- //
    private readonly IMonitor? _monitor;

    // -- Constructor -- //
    public ModLogger(IMonitor? monitor)
    {
        _monitor = monitor;
    }

    // -- Public API -- //
    public void Trace(string eventName, string message)
    {
        Log(LogLevel.Trace, eventName, message, context: null);
    }

    public void Trace(string eventName, string context, string message)
    {
        Log(LogLevel.Trace, eventName, message, context);
    }

    public void Debug(string eventName, string message)
    {
        Log(LogLevel.Debug, eventName, message, context: null);
    }

    public void Debug(string eventName, string context, string message)
    {
        Log(LogLevel.Debug, eventName, message, context);
    }

    public void Info(string eventName, string message)
    {
        Log(LogLevel.Info, eventName, message, context: null);
    }

    public void Info(string eventName, string context, string message)
    {
        Log(LogLevel.Info, eventName, message, context);
    }

    public void Warn(string eventName, string message)
    {
        Log(LogLevel.Warn, eventName, message, context: null);
    }

    public void Warn(string eventName, string context, string message)
    {
        Log(LogLevel.Warn, eventName, message, context);
    }

    public void Error(string eventName, string message)
    {
        Log(LogLevel.Error, eventName, message, context: null);
    }

    public void Error(string eventName, string context, string message)
    {
        Log(LogLevel.Error, eventName, message, context);
    }

    // -- Private Helpers -- //
    private void Log(LogLevel level, string eventName, string message, string? context)
    {
        _monitor?.Log(FormatMessage(eventName, message, context), level);
    }

    private static string FormatMessage(string eventName, string message, string? context)
    {
        if (string.IsNullOrWhiteSpace(context))
        {
            return $"[{eventName}] {message}";
        }

        return $"[{eventName}] [{context}] {message}";
    }
}
