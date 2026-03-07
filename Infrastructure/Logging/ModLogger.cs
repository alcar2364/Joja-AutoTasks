// Purpose: Wraps SMAPI IMonitor with consistent event-tagged message formatting so lifecycle and
// startup call sites produce uniform, searchable logs.
using StardewModdingAPI;

namespace JojaAutoTasks.Infrastructure.Logging;

/// <summary>Event-tagged logging facade over SMAPI monitor output.</summary>
public class ModLogger
{
    // Dependencies
    private readonly IMonitor _monitor;

    // State

    // Constants 

    // Constructor
    public ModLogger(IMonitor monitor)
    {
        _monitor = monitor;
    }

    // Public API

    // Log methods without context
    public void Trace(string eventName, string message)
    {
        this.Log(LogLevel.Trace, eventName, message, context: null);
    }

    // Log methods with context
    public void Trace(string eventName, string context, string message)
    {
        this.Log(LogLevel.Trace, eventName, message, context);
    }

    public void Debug(string eventName, string message)
    {
        this.Log(LogLevel.Debug, eventName, message, context: null);
    }
    public void Debug(string eventName, string context, string message)
    {
        this.Log(LogLevel.Debug, eventName, message, context);
    }

    public void Info(string eventName, string message)
    {
        this.Log(LogLevel.Info, eventName, message, context: null);
    }
    public void Info(string eventName, string context, string message)
    {
        this.Log(LogLevel.Info, eventName, message, context);
    }

    public void Warn(string eventName, string message)
    {
        this.Log(LogLevel.Warn, eventName, message, context: null);
    }
    public void Warn(string eventName, string context, string message)
    {
        this.Log(LogLevel.Warn, eventName, message, context);
    }

    public void Error(string eventName, string message)
    {
        this.Log(LogLevel.Error, eventName, message, context: null);
    }

    public void Error(string eventName, string context, string message)
    {
        this.Log(LogLevel.Error, eventName, message, context);
    }










    // Event Handlers

    // Private Helpers

    // Centralized log method to ensure consistent formatting
    private void Log(LogLevel level, string eventName, string message, string? context)
    {
        _monitor.Log(FormatMessage(eventName, message, context), level);
    }

    // Formats the log message to include the event name and optional context for better traceability.
    private static string FormatMessage(string eventName, string message, string? context)
    {
        if (string.IsNullOrWhiteSpace(context))
        {
            return $"[{eventName}] {message}";
        }

        return $"[{eventName}] [{context}] {message}";
    }
}