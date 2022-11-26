using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salix.StackTracer.Tests;
public class StackTraceFrameToStringTests
{
    [Fact]
    public void Empty_MethodName()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod" };
        testable.ToString().Should().Be("SomeMethod()");
    }

    [Fact]
    public void LineNumber_Added()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod", LineNumber = 12 };
        testable.ToString().Should().Be("SomeMethod(); Line:12");
    }

    [Fact]
    public void FilePath_Added()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod", FilePath = "\\MySolution\\Subfolder\\MyClass.cs"};
        testable.ToString().Should().Be("\\MySolution\\Subfolder\\MyClass.cs; SomeMethod()");
    }

    [Fact]
    public void ContainingType_Added()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod", ContainingType = typeof(StackTraceFrame)};
        testable.ToString().Should().Be("Salix.StackTracer.StackTraceFrame.SomeMethod()");
    }

    [Fact]
    public void ContainingTypeAndPath_OnlyPathAdded()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod", FilePath = "\\MySolution\\Subfolder\\MyClass.cs", ContainingType = typeof(StackTraceFrame) };
        testable.ToString().Should().Be("\\MySolution\\Subfolder\\MyClass.cs; SomeMethod()");
    }

    [Fact]
    public void GenericArgs_Added()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod" };
        testable.GenericArguments.Add(new MethodArgument { Name = "T", Type = typeof(int), TypeName = "TType" });
        testable.ToString().Should().Be("SomeMethod<TType>()");
    }

    [Fact]
    public void Parameteres_Added()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod" };
        testable.Parameters.Add(new MethodArgument { Name = "id", Type = typeof(int), TypeName = "int" });
        testable.Parameters.Add(new MethodArgument { Name = "start", Type = typeof(DateTime), TypeName = "DateTime" });
        testable.ToString().Should().Be("SomeMethod(int id, DateTime start)");
    }

    [Fact]
    public void ColumnNumber_Added()
    {
        var testable = new StackTraceFrame { MethodName = "SomeMethod", LineNumber = 12, ColumnNumber = 32 };
        testable.ToString().Should().Be("SomeMethod(); Line:12 (Col:32)");
    }
}
