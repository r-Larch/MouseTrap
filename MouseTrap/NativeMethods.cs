using System;
using System.Runtime.InteropServices;


namespace MouseTrap {
    internal static class NativeMethods {
        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr handle, WindowShowState showState);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
    }

    public enum WindowShowState : int {
        Hide = 0,
        Shownormal = 1,
        Showminimized = 2,
        Maximize = 3,
        Showmaximized = 3,
        Shownoactivate = 4,
        Show = 5,
        Minimize = 6,
        Showminnoactive = 7,
        ShowNa = 8,
        Restore = 9,
        Showdefault = 10,
        Forceminimize = 11,
    }
}
