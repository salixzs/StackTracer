using Salix.StackTracer.TestClasses;

namespace Salix.StackTracer.Tests;

public class ParserTests
{
    [Fact]
    public void JustException_NoStackTrace_EmptyList()
    {
        var ex = new Exception("Too simple");
        var testable = ex.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(0);
    }

    [Fact]
    public void Exception_SingleStackTrace_OneReturned()
    {
        ApplicationException exc;
        try
        {
            // To get stack trace
            var prepared = new ApplicationException("Testable problem");
            throw prepared;
        }
        catch (ApplicationException e)
        {
            exc = e;
        }

        exc.Should().NotBeNull();
        var testable = exc.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(1);
        testable[0].MethodName.Should().Be("Exception_SingleStackTrace_OneReturned");
        testable[0].Parameters.Should().HaveCount(0);
        testable[0].GenericArguments.Should().HaveCount(0);
    }

    [Fact]
    public void Exception_Framework_ChainReturned()
    {
        Exception exc;
        try
        {
            new TestableMethods().Math("0");
            exc = new ApplicationException("NO No NO!");
        }
        catch (DivideByZeroException e)
        {
            exc = e;
        }
        catch (ApplicationException e)
        {
            exc = e;
        }

        exc.Should().NotBeNull();
        var testable = exc.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(4);
        testable[0].FilePath.Should().BeNull();
        testable[1].FilePath.Should().BeNull();
        testable[2].MethodName.Should().Be("Math");
        testable[3].MethodName.Should().Be("Exception_Framework_ChainReturned");
    }

    [Fact]
    public void Method_NoParameters()
    {
        ApplicationException exc;
        try
        {
            new TestableMethods().Normal();
            exc = new ApplicationException("NO No NO!");
        }
        catch (ApplicationException e)
        {
            exc = e;
        }

        exc.Should().NotBeNull();
        var testable = exc.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(3);
        testable[0].MethodName.Should().Be("Throwing");
        testable[1].MethodName.Should().Be("Normal");
        testable[1].GenericArguments.Should().HaveCount(0);
        testable[1].Parameters.Should().HaveCount(0);
        testable[2].MethodName.Should().Be("Method_NoParameters");
    }

    [Fact]
    public void Method_Parameters()
    {
        ApplicationException exc;
        try
        {
            new TestableMethods().WithSimpleParameters(11, "Purr");
            exc = new ApplicationException("NO No NO!");
        }
        catch (ApplicationException e)
        {
            exc = e;
        }

        exc.Should().NotBeNull();
        var testable = exc.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(3);
        testable[0].MethodName.Should().Be("Throwing");
        testable[1].MethodName.Should().Be("WithSimpleParameters");
        testable[2].MethodName.Should().Be("Method_Parameters");
        testable[1].Parameters.Should().HaveCount(2);
        testable[1].Parameters[0].TypeName.Should().Be("int");
        testable[1].Parameters[0].Name.Should().Be("someId");
        testable[1].Parameters[1].TypeName.Should().Be("string");
        testable[1].Parameters[1].Name.Should().Be("someName");
    }

    [Fact]
    public void Method_NullParameters()
    {
        ApplicationException exc;
        try
        {
            new TestableMethods().WithNullableParameters(11, null, null);
            exc = new ApplicationException("NO No NO!");
        }
        catch (ApplicationException e)
        {
            exc = e;
        }

        exc.Should().NotBeNull();
        var testable = exc.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(3);
        testable[0].MethodName.Should().Be("Throwing");
        testable[1].MethodName.Should().Be("WithNullableParameters");
        testable[2].MethodName.Should().Be("Method_NullParameters");
        testable[1].Parameters.Should().HaveCount(3);
        testable[1].Parameters[0].TypeName.Should().Be("Nullable<int>");
        testable[1].Parameters[0].Name.Should().Be("optionalId");
        testable[1].Parameters[1].TypeName.Should().Be("Nullable<bool>");
        testable[1].Parameters[1].Name.Should().Be("triState");
        testable[1].Parameters[2].TypeName.Should().Be("string");
        testable[1].Parameters[2].Name.Should().Be("optionalName");
    }

    [Fact]
    public void Method_Generic()
    {
        ApplicationException exc;
        try
        {
            new TestableMethods().GenericMethod<int>(11);
            exc = new ApplicationException("NO No NO!");
        }
        catch (ApplicationException e)
        {
            exc = e;
        }

        exc.Should().NotBeNull();
        var testable = exc.ParseStackTrace();
        testable.Should().NotBeNull();
        testable.Should().HaveCount(3);
        testable[0].MethodName.Should().Be("Throwing");
        testable[1].MethodName.Should().Be("GenericMethod");
        testable[2].MethodName.Should().Be("Method_Generic");
        testable[1].Parameters.Should().HaveCount(1);
        testable[1].Parameters[0].TypeName.Should().Be("TParm");
        testable[1].Parameters[0].Name.Should().Be("typedParm");
        testable[1].GenericArguments.Should().HaveCount(1);
        testable[1].GenericArguments[0].TypeName.Should().Be("TParm");
    }
}