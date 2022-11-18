using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Salix.StackTracer;

/// <summary>
/// Parsed, cleaned up and simplified stack trace frame (one line).
/// </summary>
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
    public Type? ContainingType { get; set; }

    /// <summary>
    /// Method (code piece) which is part of stack trace.
    /// </summary>
    public required string MethodName { get; set; }

    /// <summary>
    /// In case <see cref="MethodName"/> is a generic - here are generic arguments (not actual types, but "T"s).
    /// </summary>
    public List<MethodArgument> GenericArguments { get; set; } = new List<MethodArgument>();

    /// <summary>
    /// Parameter types and names used in <see cref="MethodName">Method</see> call
    /// (Types and Names - without actual values!)
    /// </summary>
    public List<MethodArgument> Parameters { get; set; } = new List<MethodArgument>();

    /// <summary>
    /// Line number in code which is involved in stack trace (in Exception - calling code or line actualy caused exception).
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Column number (within <see cref="LineNumber">Line number</see>) in code which is involved in stack trace
    /// (in Exception - calling code or line actualy caused exception).
    /// </summary>
    public int ColumnNumber { get; set; }

    /// <summary>
    /// String concatenating File and method information for simplified and more readable stack trace frame.<br/>
    /// Can be used directly in logging statements.
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
            var genericArguments = string.Join(", ", this.GenericArguments.Select(arg => arg.TypeName));
            sb.Append($"<{genericArguments}>");
        }

        if (Parameters.Count > 0)
        {
            var methodParameters = string.Join(", ", this.Parameters.Select(arg => arg.ToString()));
            sb.Append($"({methodParameters})");
        }
        else
        {
            sb.Append("()");
        }

        if (LineNumber != 0)
        {
            sb.Append($"; Line:{LineNumber:D}");
        }

        if (ColumnNumber != 0)
        {
            sb.Append($" (Col:{ColumnNumber:D})");
        }

        return sb.ToString();
    }

    [ExcludeFromCodeCoverage]
    private string GetDebuggerDisplay() => ToString();
}

/// <summary>
/// Description of Method's generic argument or parameter.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class MethodArgument
{
    /// <summary>
    /// Type of the parameter.
    /// </summary>
    public required Type Type { get; set; }

    /// <summary>
    /// Type name 
    /// </summary>
    public required string TypeName { get; set; }

    /// <summary>
    /// Name of generic argument or parameter.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// For parameters flag, indicating whether it is OUT.
    /// </summary>
    public bool IsOut { get; set; }

    /// <summary>
    /// For parameters flag, indicating whether it is REF parameter.
    /// </summary>
    public bool IsRef { get; set; }

    /// <summary>
    /// String representation of generic argument or parameter.
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        $"{(IsOut ? "out " : string.Empty)}{(IsRef ? "ref " : string.Empty)}{TypeName} {Name}";

    [ExcludeFromCodeCoverage]
    private string GetDebuggerDisplay() => ToString();
}
