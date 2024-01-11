using System.Diagnostics;

namespace Salix.StackTracer;

/// <summary>
/// Internal frame, used to handle both custom frame object together with original frame.
/// </summary>
internal class InternalStackTraceFrame
{
    public string? FilePath { get; internal set; }

    public Type? ContainingType { get; internal set; }

    public required string MethodName { get; internal set; }

    public List<MethodArgument> GenericArguments { get; internal set; } = new List<MethodArgument>();

    public List<MethodArgument> Parameters { get; internal set; } = new List<MethodArgument>();

    public int LineNumber { get; internal set; }

    public int ColumnNumber { get; internal set; }

    public StackFrame OriginalFrame { get; internal set; } = null!;
}
