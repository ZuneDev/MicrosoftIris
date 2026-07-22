using System;
using System.IO;

namespace Microsoft.Iris.Render.Subsystems.Tracing;

// Real state for the whole tracing surface: SpInitializeTracing/SpUninitializeTracing
// (init flag) plus SpUpdateTraceSettings/SpLogTrace (settings + line formatting/sink).
internal static class TracingState
{
    public static bool IsInitialized { get; private set; }

    private static string s_debugTraceFile;
    private static string s_writeLinePrefix = "";
    private static bool s_sendOutputToDebugger;
    private static bool s_showCategories;
    private static bool s_timedWriteLines;
    private static readonly object s_fileLock = new();

    public static void Initialize() => IsInitialized = true;
    public static void Uninitialize() => IsInitialized = false;

    public static void UpdateSettings(string debugTraceFile, string writeLinePrefix, bool sendOutputToDebugger, bool showCategories, bool timedWriteLines)
    {
        s_debugTraceFile = debugTraceFile;
        s_writeLinePrefix = writeLinePrefix ?? "";
        s_sendOutputToDebugger = sendOutputToDebugger;
        s_showCategories = showCategories;
        s_timedWriteLines = timedWriteLines;
    }

    public static void LogTrace(string categoryName, string message, int indentLevel)
    {
        if (!IsInitialized)
            return;

        var line = new System.Text.StringBuilder();
        line.Append(s_writeLinePrefix);
        if (s_timedWriteLines)
            line.Append('[').Append(DateTime.Now.ToString("HH:mm:ss.fff")).Append("] ");
        if (s_showCategories && !string.IsNullOrEmpty(categoryName))
            line.Append('[').Append(categoryName).Append("] ");
        line.Append(' ', indentLevel * 2);
        line.Append(message);

        string text = line.ToString();

        if (s_sendOutputToDebugger)
            Console.Error.WriteLine(text);

        if (!string.IsNullOrEmpty(s_debugTraceFile))
        {
            lock (s_fileLock)
            {
                try
                {
                    File.AppendAllText(s_debugTraceFile, text + Environment.NewLine);
                }
                catch (IOException)
                {
                    // Best-effort trace sink -- matches the original's fire-and-forget
                    // SpLogTrace, which has no HRESULT to report failure through either.
                }
            }
        }
    }
}
