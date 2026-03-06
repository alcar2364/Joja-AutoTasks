using StardewModdingAPI;

namespace JojaAutoTasks.Infrastructure.Logging;

/// <summary> 
/// Provides a simple wrapper around SMAPI's IMonitor for consistent log formatting and event ID usage throughout the mod.
/// </summary>
public class ModLogger
{
    // Dependencies
    private readonly IMonitor monitor;

    // Constructor
    public ModLogger(IMonitor monitor)
    {
        this.monitor = monitor;
    }

    // Public APIs
    
    // Log methods without context
    public void Trace(string eventName, string message)
    {
        this.Log(LogLevel.Trace, eventName, message, context: null);
    }

    public void Debug(string eventName, string message)
    {
        this.Log(LogLevel.Debug, eventName, message, context: null);
    }

    public void Info(string eventName, string message)
    {
        this.Log(LogLevel.Info, eventName, message, context: null);
    }

    public void Warn(string eventName, string message)
    {
        this.Log(LogLevel.Warn, eventName, message, context: null);
    }

    public void Error(string eventName, string message)
    {
        this.Log(LogLevel.Error, eventName, message, context: null);
    }

    // Log methods with context
    public void Trace(string eventName, string context, string message)
    {
        this.Log(LogLevel.Trace, eventName, message, context);
    }

    public void Debug(string eventName, string context, string message)
    {
        this.Log(LogLevel.Debug, eventName, message, context);
    }

    public void Info(string eventName, string context, string message)
    {
        this.Log(LogLevel.Info, eventName, message, context);
    }

    public void Warn(string eventName, string context, string message)
    {
        this.Log(LogLevel.Warn, eventName, message, context);
    }

    public void Error(string eventName, string context, string message)
    {
        this.Log(LogLevel.Error, eventName, message, context);
    }

    // Private Helpers

    // Centralized log method to ensure consistent formatting
    private void Log(LogLevel level, string eventName, string message, string? context)
    {
        monitor.Log(FormatMessage(eventName, message, context), level);
    }

    // Formats the log message to include the event name and optional context for better traceability.
    private string FormatMessage(string eventName, string message, string? context)
    {
        if (string.IsNullOrWhiteSpace(context))
        {
            return $"[{eventName}] {message}";
        }
        
            return $"[{eventName}] [{context}] {message}";
    }
}