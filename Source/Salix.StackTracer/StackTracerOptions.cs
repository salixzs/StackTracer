namespace Salix.StackTracer;

/// <summary>
/// Possible options to filter parsed stack trace frames.
/// </summary>
public class StackTracerOptions
{
    /// <summary>
    /// .Net Framework frames normally doesn't have line numbers (if there is not special DBG symbol copying involved). <br/>
    /// Setting this to true normally returns only code frames having line numbers (== having DBG, which means - your own code).<br/>
    /// WARNING: If you deliberately prohibit deploying DBG files in Production - also your code lines will be 0 (filtered out).
    /// </summary>
    public bool SkipFramesWithoutLineNumber { get; set; }

    /// <summary>
    /// Supply a list of strings, which will filter out frames, having/containing these in Path, Namespace or Method name.<br/>
    /// Default: Empty list, which includes all frames.
    /// </summary>
    public HashSet<string> SkipFramesContaining { get; set; } = new HashSet<string>();

    /// <summary>
    /// Whitelisting only frames, having specified string(s) in their namespace.<br/>
    /// All other frames will not be included.<br/>
    /// Similar to <see cref="SkipFramesWithoutLineNumber"/> is an another way to show only your own code.
    /// </summary>
    public HashSet<string> ShowOnlyFramesWithNamespace { get; set; } = new HashSet<string>();

    internal bool HasWhitelist => ShowOnlyFramesWithNamespace.Count > 0;
    internal bool HasBlacklist => SkipFramesContaining.Count > 0;

    internal bool IsWhitelisted(StackTraceFrame stackTraceFrame)
    {
        if (!HasWhitelist || string.IsNullOrEmpty(stackTraceFrame.ContainingType?.Namespace))
        {
            return false;
        }

        if (ShowOnlyFramesWithNamespace.Any(whitelisted => stackTraceFrame.ContainingType!.Namespace.Contains(whitelisted, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }

    internal bool IsBlacklisted(StackTraceFrame stackTraceFrame)
    {
        if (!HasBlacklist)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(stackTraceFrame.FilePath) && SkipFramesContaining.Any(blacklisted =>
            stackTraceFrame.FilePath!.Contains(blacklisted, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(stackTraceFrame.ContainingType?.Namespace) && SkipFramesContaining.Any(blacklisted =>
            stackTraceFrame.ContainingType!.Namespace.Contains(blacklisted, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(stackTraceFrame.ContainingType?.Name) && SkipFramesContaining.Any(blacklisted =>
            stackTraceFrame.ContainingType!.Name.Contains(blacklisted, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(stackTraceFrame.MethodName) && SkipFramesContaining.Any(blacklisted =>
            stackTraceFrame.MethodName.Contains(blacklisted, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }
}
