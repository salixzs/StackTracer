using System.Diagnostics;
using System.Text;

namespace Salix.StackTracer;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StackTraceFrame
{
    /// <summary>
    /// Relative path to Class file, containing both <see cref="ContainingType"/> and its <see cref="MethodName"/>.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Class, containing <see cref="MethodName"/>, which is part of Stack trace.
    /// </summary>
    public Type? ContainingType { get; set; } = null;

    /// <summary>
    /// Method (code piece) which is part of stack trace.
    /// </summary>
    public required string MethodName { get; set; }

    /// <summary>
    /// In case <see cref="MethodName"/> is a generic - here are generic arguments (not actual types, but "T"s).
    /// </summary>
    public List<MethodArgument> GenericArguments { get; set; } = new List<MethodArgument>();

    /// <summary>
    /// Parameter types and names used in <see cref="MethodName">Method</see> call (No actual values!)
    /// </summary>
    public List<MethodArgument> Parameters { get; set; } = new List<MethodArgument>();

    /// <summary>
    /// Line number in code which is involved in stack trace (in Exception - passing or throwing).
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Column number (within <see cref="LineNumber">Line number</see>) in code which is involved in stack trace (for Exception - calling next line or problem line).
    /// </summary>
    public int ColumnNumber { get; set; }

    /// <summary>
    /// String concatenating File and method information for simplified and more readable stack trace frame.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (FilePath != null)
        {
            sb.Append(FilePath);
            sb.Append("; ");
        }
        else if (ContainingType != null)
        {
            sb.Append(ContainingType.FullName);
            sb.Append("; ");
        }

        sb.Append(this.MethodName);
        if (GenericArguments.Count > 0)
        {
            string genericArguments = string.Join(", ", this.GenericArguments.Select(arg => arg.TypeName));
            sb.Append($"<{genericArguments}>");
        }

        if (Parameters.Count > 0)
        {
            string methodParameters = string.Join(", ", this.Parameters.Select(arg => arg.ToString()));
            sb.Append($"({methodParameters})");
        }
        else
        {
            sb.Append("()");
        }

        if (LineNumber != null)
        {
            sb.Append($"; Line:{LineNumber:D}");
        }

        if (ColumnNumber != null)
        {
            sb.Append($" (Col:{ColumnNumber:D})");
        }

        return sb.ToString();
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class MethodArgument
{
    public required Type Type { get; set; }
    public required string TypeName { get; set; }
    public required string Name { get; set; }
    public bool IsOut { get; set; }
    public bool IsRef { get; set; }

    public override string ToString()
    {
        return $"{(IsOut ? "out " : string.Empty)}{(IsRef ? "ref " : string.Empty)}{TypeName} {Name}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}