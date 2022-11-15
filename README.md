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

> TBC.

> NuGet package TBC[reated]
