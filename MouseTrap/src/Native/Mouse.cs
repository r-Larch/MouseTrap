using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;


namespace MouseTrap.Native {
    internal class Mouse {
        public static void MoveCursor(int x, int y)
        {
            Win32.SetCursorPos(x, y);
        }

        public static Rectangle Clip {
            get {
                var rect = new Win32.RECT();
                if (Win32.GetClipCursor(rect)) {
                    return rect;
                }
                else {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }
            }
            set {
                if (!Win32.ClipCursor(value.IsEmpty ? null : (Win32.RECT) value)) {
                    var error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }
            }
        }

        public static void SwitchToInputDesktop()
        {
            var threadCurrent = GetCurrentDesktop();
            var inputCurrent = GetInputDesktop();

            if (threadCurrent != inputCurrent) {
                SetCurrentDesktop(inputCurrent);
            }
            else {
                Win32.CloseDesktop(inputCurrent);
            }
        }

        private static IntPtr GetCurrentDesktop()
        {
            var hDesktop = Win32.GetThreadDesktop(Win32.GetCurrentThreadId());
            var error = Marshal.GetLastWin32Error();
            if (error != 0) {
                throw new Win32Exception(error);
            }

            return hDesktop;
        }

        private static void SetCurrentDesktop(IntPtr desktopHandle)
        {
            var failed = Win32.SetThreadDesktop(desktopHandle);
            if (!failed) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static IntPtr GetInputDesktop()
        {
            var hDesktop = Win32.OpenInputDesktop(dwFlags: 0, fInherit: false, dwDesiredAccess: 0);
            var error = Marshal.GetLastWin32Error();
            if (error != 0) {
                throw new Win32Exception(error);
            }

            return hDesktop;
        }
    }
}
