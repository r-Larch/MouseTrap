using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using MouseTrap.Models;


namespace MouseTrap;

internal class Logger : SettingsFile {
    private readonly string _datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    private readonly object _fileLock = new();
    public readonly string LogFilename;

    public static readonly Logger Log = new();

    public Logger()
    {
        LogFilename = Path.ChangeExtension(SavePath($"{App.Name}"), ".log");

        var logHeader = LogFilename + " is created.";
        if (!File.Exists(LogFilename)) {
            WriteLine(DateTime.Now.ToString(_datetimeFormat) + " " + logHeader, false);
        }
    }

    /// <summary>
    /// Log a DEBUG message
    /// </summary>
    /// <param name="text">Message</param>
    public void Debug(string text)
    {
        WriteFormattedLog(LogLevel.Debug, text);
    }

    /// <summary>
    /// Log an ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    public void Error(string text)
    {
        WriteFormattedLog(LogLevel.Error, text);
    }


    public static void Error(string message, Exception? e)
    {
        var msg = new StringBuilder();
        if (e is null) msg.Append(message);
        while (e != null) {
            var hResult = (e is Win32Exception win32) ? win32.NativeErrorCode : e.HResult;
            msg.AppendLine($"[0x{0x80070000 + hResult:X}] {e.GetType().FullName}: {e.Message}\r\n{e.StackTrace}");
            e = e.InnerException;
        }

        Log.Error(msg.ToString());

        if (OperatingSystem.IsWindows()) {
            EventLog.WriteEntry(App.Name, msg.ToString(), EventLogEntryType.Error);
        }
    }

    /// <summary>
    /// Log a FATAL ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    public void Fatal(string text)
    {
        WriteFormattedLog(LogLevel.Fatal, text);
    }

    /// <summary>
    /// Log an INFO message
    /// </summary>
    /// <param name="text">Message</param>
    public void Info(string text)
    {
        WriteFormattedLog(LogLevel.Info, text);
    }

    /// <summary>
    /// Log a TRACE message
    /// </summary>
    /// <param name="text">Message</param>
    public void Trace(string text)
    {
        WriteFormattedLog(LogLevel.Trace, text);
    }

    /// <summary>
    /// Log a WARNING message
    /// </summary>
    /// <param name="text">Message</param>
    public void Warning(string text)
    {
        WriteFormattedLog(LogLevel.Warning, text);
    }

    private void WriteLine(string text, bool append = true)
    {
        if (!string.IsNullOrEmpty(text)) {
            lock (_fileLock) {
                try {
                    var file = new FileInfo(LogFilename);
                    if (file.Exists && file.Length > (1024 * 1024 * 3)) {
                        file.Delete();
                    }

                    using var fs = new FileStream(LogFilename, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
                    using var writer = new StreamWriter(fs, Encoding.UTF8);
                    writer.WriteLine(text);
                }
                catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }
    }

    private void WriteFormattedLog(LogLevel level, string text)
    {
        var pretext = level switch {
            LogLevel.Trace => System.DateTime.Now.ToString(_datetimeFormat) + " [TRACE]   ",
            LogLevel.Info => System.DateTime.Now.ToString(_datetimeFormat) + " [INFO]    ",
            LogLevel.Debug => System.DateTime.Now.ToString(_datetimeFormat) + " [DEBUG]   ",
            LogLevel.Warning => System.DateTime.Now.ToString(_datetimeFormat) + " [WARNING] ",
            LogLevel.Error => System.DateTime.Now.ToString(_datetimeFormat) + " [ERROR]   ",
            LogLevel.Fatal => System.DateTime.Now.ToString(_datetimeFormat) + " [FATAL]   ",
            _ => ""
        };

        WriteLine(pretext + text);
    }

    private enum LogLevel {
        Trace,
        Info,
        Debug,
        Warning,
        Error,
        Fatal
    }
}
