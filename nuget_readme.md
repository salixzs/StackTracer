# Salix.StackTracer

Extensions methods to `StackTrace` and `Exception` (any derived exception, too) objects to get more simplified and usable stack trace frame list, wrapped in custom object list.

```c#
// omitted for brevity
catch (Exception ex)
{
    var simpleFrames = ex.ParseStackTrace(); // <---- This is the extension method
    logger.LogError(simpleFrames[0].ToString()); // <--- oversimplified usage
}
```

StackTrace frames can be filtered with several filter approaches to leave only frames from own code and drop everything passed through .Net framework and external package functionalities,
where normally developers does not have direct control over (and 99% times is not cause for thrown exception).

Overriden `ToString()` for returned `StackTraceFrame` objects can be used to show cleaned up and more readable stack trace output (for logging).

```
Salix.StackTracer.Tests\TestableMethods.cs; Throwing(); Line:30 (Col:33)
Salix.StackTracer.Tests\TestableMethods.cs; WithNullableParameters(Nullable<int> optionalId, Nullable<bool> triState, string optionalName); Line:23 (Col:9)
Salix.StackTracer.Tests\ParserTests.cs; Method_NullParameters(); Line:128 (Col:13)
```

If you want to format or colorize outout yourself, [`StackTraceFrame`](https://github.com/salixzs/StackTracer/blob/main/Source/Salix.StackTracer/StackTraceFrame.cs) has properties containing parts of this output as separate types/strings/objects.

`StackTraceFrame` contains:
- Method name
  - Collection of parameters of the method (empty if method does not have parameters)
  - Collection of generic types, if method is generic one.
- Containing type (Class, which defines this method)
- Code file path as string (shortened, relative)
- Line number
  - Column number

StackTraceFrame does not provide values, passed into method as parameters, as this information is not available in .Net framework Stack Trace.

## Filtering

If you are interested only in your own code to be shown in stack trace (skip all .Net code and external dependencies code), there are options to be set
when `ParseStackTrace()` extensions methods are invoked.

### SkipFramesWithoutLineNumber

```c#
// omitted for brevity
catch (Exception ex)
{
    var simpleFrames = ex.ParseStackTrace(new StackTracerOptions { SkipFramesWithoutLineNumber = true });
    // -OR -
    var simpleFrames = ex.ParseStackTrace(o => o.SkipFramesWithoutLineNumber = true);
}
```

This will skip all stack frames where line number is not retrieved. It happens for assemblies which does not have PDB files available (normally framework and added NuGet packages)
and is the simplest way to filter out these dependencies.

**NOTE:** If you forcibly remove PDB files in your deployed application for your own code, if might return empty collection of stack frames,
as your own code will not have line number, too.

### ShowOnlyFramesWithNamespace

```c#
// omitted for brevity
catch (Exception ex)
{
    var simpleFrames = ex.ParseStackTrace(new StackTracerOptions { ShowOnlyFramesWithNamespace = new HashSet<string> { "MyApp." } });
    // -OR -
    var simpleFrames = ex.ParseStackTrace(o => o.ShowOnlyFramesWithNamespace = new HashSet<string> { "MyApp." });
}
```

This will skip (whitelist) all stack frames where method's containing type does not contain one of given strings in options.

### SkipFramesContaining

```c#
// omitted for brevity
catch (Exception ex)
{
    var simpleFrames = ex.ParseStackTrace(new StackTracerOptions
    {
        ShowOnlyFramesWithNamespace = new HashSet<string> { "MyApp." },
        SkipFramesContaining = new HashSet<string> { "ButBlackistedExact" }
    });

    // - OR -

    var simpleFrames = exc.ParseStackTrace(opts =>
    {
        opts.ShowOnlyFramesWithNamespace = new HashSet<string> { "MyApp." };
        opts.SkipFramesContaining = new HashSet<string> { "ButBlackistedExact" };
    });
}
```

This will skip (blacklist) all stack frames where either namespace of method's containing type or method name contains one of given strings in options.
Can be used to skip general filters, middleware etc. which usually cannot be cause for exception, but is part of your own code.



## Cleanup original StackTrace

Exception has an extension method to get back a list of StackFrames (.Net objects) which are filtered based on options and file paths shortened.

This can come handy in case there is a need to have .Net StackTrace in their original object types.


```c#
// omitted for brevity
catch (Exception ex)
{
    var filteredFrames = ex.FilteredStackTrace(new StackTracerOptions { SkipFramesWithoutLineNumber = true });
}
```
