using System.Diagnostics;
using System.Reflection;

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
    public static List<StackTraceFrame> Parse(this StackTrace stackTrace, StackTracerOptions? options = null)
    {
        var internalFrames = StackTraceParser.GetStackTrace(stackTrace, options ?? new StackTracerOptions());
        return MapToPublicCustomFrames(internalFrames);
    }

    private static List<StackTraceFrame> MapToPublicCustomFrames(List<InternalStackTraceFrame> internalFrames) =>
        internalFrames.Select(f => new StackTraceFrame
        {
            MethodName = f.MethodName,
            FilePath = f.FilePath,
            ContainingType = f.ContainingType,
            LineNumber = f.LineNumber,
            ColumnNumber = f.ColumnNumber,
            GenericArguments = f.GenericArguments,
            Parameters = f.Parameters
        }).ToList();

    /// <summary>
    /// Parses, transforms and cleans up <see cref="StackTrace">StackTrace</see>
    /// within <see cref="Exception">Exception</see> (and all derived exception classes)
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

    /// <summary>
    /// Parses, transforms and cleans up <see cref="StackTrace">StackTrace</see>
    /// within <see cref="Exception">Exception</see> (and all derived exception classes)
    /// to be better usable and readable (for logging).
    /// <code>
    /// var cleaned = exc.ParseStackTrace(opts => opts.SkipFramesWithoutLineNumber = true);
    /// </code>
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="options">Options to control cleanup behavior.</param>
    /// <returns>List of stack trace frames with more readable and usable contents.</returns>
    public static List<StackTraceFrame> ParseStackTrace(this Exception exception, Action<StackTracerOptions> setupAction)
    {
        var options = new StackTracerOptions();
        setupAction(options);
        return ParseStackTrace(exception, options);
    }

    /// <summary>
    /// Parses, transforms and cleans up <see cref="StackTrace">StackTrace</see>
    /// within <see cref="Exception">Exception</see> (and all derived exception classes)
    /// to be better usable and readable.
    /// </summary>
    /// <param name="exception">Exception object.</param>
    /// <param name="options">Options to control cleanup behavior.</param>
    /// <returns>List of .Net filtered original stack frames with shortened file paths.</returns>
    public static List<StackFrame> FilteredStackTrace(this Exception exception, StackTracerOptions? options = null)
    {
        var stackTrace = new StackTrace(exception, true);
        var internalFrames = StackTraceParser.GetStackTrace(stackTrace, options ?? new StackTracerOptions());

        var filteredFrames = new List<StackFrame>();
        foreach (var frame in internalFrames)
        {
            filteredFrames.Add(UpdatePath(frame.OriginalFrame, frame.FilePath));
        }

        return filteredFrames;
    }

    private static StackFrame UpdatePath(StackFrame originalFrame, string? filePath)
    {
        var pathField = typeof(StackFrame).GetField("_fileName", BindingFlags.NonPublic | BindingFlags.Instance);
        pathField?.SetValue(originalFrame, filePath);
        return originalFrame;
    }
}
