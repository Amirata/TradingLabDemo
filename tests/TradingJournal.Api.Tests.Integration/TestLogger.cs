using Microsoft.Extensions.Logging;

namespace TradingJournal.Api.Tests.Integration;

public class TestLogger : ILogger
{
    private readonly List<string> _logMessages = new();

    public IReadOnlyList<string> LogMessages => _logMessages;

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        // Use the formatter to include both the state and the exception
        var message = formatter(state, exception);

        // Add the formatted message to the log collection
        _logMessages.Add(message);
        
    }
    
    public void Clear()
    {
        _logMessages.Clear();
    }
    
}

public class TestLoggerProvider : ILoggerProvider
{
    private readonly TestLogger _testLogger = new();

    public TestLogger Logger => _testLogger;

    public ILogger CreateLogger(string categoryName) => _testLogger;

    public void Dispose() { }
    
    public void ClearLogs()
    {
        _testLogger.Clear();
    }
}
