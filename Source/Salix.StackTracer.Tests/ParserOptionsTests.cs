using Salix.StackTracer.TestClasses;

namespace Salix.StackTracer.Tests;

public class ParserOptionTests
{
    [Fact]
    public void Options_OnlyWithLineNumbers()
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
        var testable = exc.ParseStackTrace(new StackTracerOptions { SkipFramesWithoutLineNumber = true });
        testable.Should().NotBeNull();
        testable.Should().HaveCount(2);
        testable[0].MethodName.Should().Be("Math");
        testable[1].MethodName.Should().Be("Options_OnlyWithLineNumbers");
    }

    [Fact]
    public void Options_WhitelistNamespace()
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
        var testable = exc.ParseStackTrace(new StackTracerOptions { ShowOnlyFramesWithNamespace = new HashSet<string> { "Salix." } });
        testable.Should().NotBeNull();
        testable.Should().HaveCount(2);
        testable[0].MethodName.Should().Be("Math");
        testable[1].MethodName.Should().Be("Options_WhitelistNamespace");
    }

    [Fact]
    public void Options_WhitelistExactNamespace()
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
        var testable = exc.ParseStackTrace(new StackTracerOptions { ShowOnlyFramesWithNamespace = new HashSet<string> { "Salix.StackTracer.Tests" } });
        testable.Should().NotBeNull();
        testable.Should().HaveCount(1);
        testable[0].MethodName.Should().Be("Options_WhitelistExactNamespace");
    }

    [Fact]
    public void Options_WhitelistRootNamespace_ButBlackistedExact()
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
        var testable = exc.ParseStackTrace(new StackTracerOptions
            {
                ShowOnlyFramesWithNamespace = new HashSet<string> { "Salix." },
                SkipFramesContaining = new HashSet<string> { "ButBlackistedExact" }
            });
        testable.Should().NotBeNull();
        testable.Should().HaveCount(1);
        testable[0].MethodName.Should().Be("Math");
    }

}