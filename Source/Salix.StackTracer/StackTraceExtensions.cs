using System.Diagnostics;

namespace Salix.StackTracer;

/// <summary>
/// Extensions to <see cref="StackTrace">StackTrace</see> and <see cref="Exception">Exception</see>.
/// </summary>
public static class StackTraceExtensions
{
    /// <summary>
    /// Parses, transforms and cleans up <see cref="StackTrace">StackTrace</see>
    /// to be better usable and readable (for logging).
    /// </summary>
    /// <param name="stackTrace">Original Stack trace object.</param>
    /// <param name="options">Options to control cleanup behavior.</param>
    /// <returns>List of stack trace frames with more readable and usable contents.</returns>
    public static List<StackTraceFrame> Parse(this StackTrace stackTrace, StackTracerOptions? options = null) =>
        StackTraceParser.GetStackTrace(stackTrace, options ?? new StackTracerOptions());

    /// <summary>
    /// Parses, transforms and cleans up <see cref="StackTrace">StackTrace</see>
    /// within <see cref="Exception">Exception</see> and all derived exception classes
    /// to be better usable and readable (for logging).
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="options">Options to control cleanup behavior.</param>
    /// <returns>List of stack trace frames with more readable and usable contents.</returns>
    public static List<StackTraceFrame> ParseStackTrace(this Exception exception, StackTracerOptions? options = null)
    {
        var stackTrace = new StackTrace(exception, true);
        return stackTrace.Parse(options);
    }
}
