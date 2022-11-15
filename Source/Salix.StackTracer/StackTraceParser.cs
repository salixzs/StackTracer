using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Salix.StackTracer;

internal static class StackTraceParser
{
    internal static List<StackTraceFrame> GetStackTrace(StackTrace stackTrace, StackTracerOptions options)
    {
        var frames = new List<StackTraceFrame>();
        var stackFrames = stackTrace.GetFrames();
        if (stackFrames == null)
        {
            return frames;
        }

        // Common path parts (root folder etc.) is filled in loop below -
        // to remove them later to shorten stack trace and increase readability.
        var folderNameOccurrences = new Dictionary<string, FolderOccurrenceCount>();
        foreach (var frame in stackFrames)
        {
            var stackTraceFrame = new StackTraceFrame
            {
                FilePath = frame.GetFileName(),
                MethodName = "?", // Filled later below
                LineNumber = frame.GetFileLineNumber(),
                ColumnNumber = frame.GetFileColumnNumber(),
            };

            if (options.SkipFramesWithoutLineNumber && stackTraceFrame.LineNumber == 0)
            {
                continue;
            }

            // Here we squeeze all juices outta method
            PopulateMethodMetadata(stackTraceFrame, frame.GetMethod());

            if (options.IsBlacklisted(stackTraceFrame))
            {
                continue;
            }

            if (options.HasWhitelist && !options.IsWhitelisted(stackTraceFrame))
            {
                continue;
            }

            frames.Add(stackTraceFrame);

            if (string.IsNullOrEmpty(stackTraceFrame.FilePath) || stackTraceFrame.FilePath.Length < 2)
            {
                continue;
            }

            // Experience shows these can be both \ and / in single StackTrace
            char directorySeparatorChar = '\\';
            if (stackTraceFrame.FilePath.IndexOf(directorySeparatorChar) < 0)
            {
                directorySeparatorChar = '/';
                if (stackTraceFrame.FilePath.IndexOf(directorySeparatorChar) < 0)
                {
                    // Only file name
                    continue;
                }
            }

            var filepathFolders = stackTraceFrame.FilePath.Split(directorySeparatorChar).ToList();
            if (filepathFolders.Count < 3)
            {
                continue;
            }

            // Remove filename and 1 folder before it from entire file path (so they are preserved)
            filepathFolders.RemoveRange(filepathFolders.Count - 2, 2);
            var nonEmptyFolderList = filepathFolders.Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
            foreach (string folderName in nonEmptyFolderList)
            {
                if (folderNameOccurrences.ContainsKey(folderName))
                {
                    folderNameOccurrences[folderName].Occurrences++;
                }
                else
                {
                    folderNameOccurrences.Add(folderName, new FolderOccurrenceCount(directorySeparatorChar));
                }
            }
        }

        // Remove folders, which are encountered only once (so they are left in stacktrace)
        var singleUseFolders = folderNameOccurrences
            .Where(o => o.Value.Occurrences <= 1)
            .ToList();
        foreach (var item in singleUseFolders)
        {
            folderNameOccurrences.Remove(item.Key);
        }

        for (var frameIndex = 0; frameIndex < frames.Count; frameIndex++)
        {
            foreach (var pathPart in folderNameOccurrences)
            {
                frames[frameIndex].FilePath = frames[frameIndex].FilePath?
                    .Replace(pathPart.Key + pathPart.Value.DirectorySeparatorChar, string.Empty);
            }
        }

        return frames;
    }

    /// <summary>
    /// Returns Method signature or null, if method is not passed in.
    /// </summary>
    /// <param name="method">Method information from reflection.</param>
    private static void PopulateMethodMetadata(StackTraceFrame frameData, MethodBase? method)
    {
        if (method == null)
        {
            frameData.MethodName = "?";
            return;
        }

        frameData.ContainingType = method.DeclaringType;
        frameData.MethodName = method.Name;

        if (frameData.ContainingType?.IsDefined(typeof(CompilerGeneratedAttribute)) == true
            && (typeof(IAsyncStateMachine).IsAssignableFrom(frameData.ContainingType) || typeof(IEnumerator).IsAssignableFrom(frameData.ContainingType)))
        {
            method = ResolveStateMachineMethod(method);
            frameData.MethodName = method.Name;
        }

        // Method generic arguments <T, T>
        if (method.IsGenericMethod)
        {
            foreach(var genericArgument in method.GetGenericArguments())
            {
                frameData.GenericArguments.Add(new MethodArgument
                {
                    Name = "T",
                    Type = genericArgument,
                    TypeName = TypeNameHelper.GetTypeDisplayName(genericArgument, false, true),
                });
            }
        }

        // Method parameters Method(type param1, type param2)
        foreach (ParameterInfo methodParameter in method.GetParameters())
        {
            var parameterDescriber = new MethodArgument
            {
                Name = methodParameter.Name ?? "?",
                Type = methodParameter.ParameterType,
                TypeName = "?"
            };

            if (methodParameter.IsOut)
            {
                parameterDescriber.IsOut = true;
            }
            else if (methodParameter.ParameterType.IsByRef)
            {
                parameterDescriber.IsRef = true;
            }

            if (parameterDescriber.IsRef)
            {
                parameterDescriber.Type = parameterDescriber.Type.GetElementType() ?? parameterDescriber.Type;
            }

            parameterDescriber.TypeName = TypeNameHelper.GetTypeDisplayName(parameterDescriber.Type, false, true);
            frameData.Parameters.Add(parameterDescriber);
        }
    }

    private static MethodBase ResolveStateMachineMethod(MethodBase method)
    {
        var declaringType = method.DeclaringType;
        var retMethod = method;
        var parentType = method.DeclaringType?.DeclaringType;
        if (parentType == null)
        {
            return retMethod;
        }

        var methods = parentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var candidateMethod in methods)
        {
            var attributes = candidateMethod.GetCustomAttributes<StateMachineAttribute>();
            foreach (var sma in attributes)
            {
                if (sma.StateMachineType == declaringType)
                {
                    return candidateMethod;
                }
            }
        }

        return retMethod;
    }

    /// <summary>
    /// Internal DTO for repeating folder counting
    /// </summary>
    private sealed class FolderOccurrenceCount
    {
        /// <summary>
        /// Holds count of folder occurrences in stack trace filePaths.
        /// </summary>
        public int Occurrences { get; set; } = 1;

        /// <summary>
        /// Stores directory separator char, used in this particular path (could be / or \).
        /// </summary>
        public char DirectorySeparatorChar { get; }

        public FolderOccurrenceCount(char directorySeparatorChar) =>
            this.DirectorySeparatorChar = directorySeparatorChar;
    }
}
