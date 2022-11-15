using System.Diagnostics;

namespace Salix.StackTracer;

public static class StackTraceExtensions
{
    public static List<StackTraceFrame> Parse(this StackTrace stackTrace, StackTracerOptions? options = null)
    {
        return StackTraceParser.GetStackTrace(stackTrace, options ?? new StackTracerOptions());
    }

    public static List<StackTraceFrame> ParseStackTrace(this Exception exception, StackTracerOptions? options = null)
    {
        var stackTrace = new StackTrace(exception, true);
        return stackTrace.Parse(options);
    }
}
