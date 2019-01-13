using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;


namespace MouseTrap {
    internal class Logger {
        public static void Error(string message, Exception e)
        {
            var msg = new StringBuilder();
            do {
                var hResult = (e is Win32Exception win32) ? win32.NativeErrorCode : e.HResult;
                msg.AppendLine($"[0x{0x80070000 + hResult:X}] {e.GetType().FullName}: {e.Message}\r\n{e.StackTrace}");
                e = e.InnerException;
            } while (e != null);

            // log
            EventLog.WriteEntry(App.Name, msg.ToString(), EventLogEntryType.Error);
        }
    }
}
