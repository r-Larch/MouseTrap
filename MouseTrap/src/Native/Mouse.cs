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

        public static Rectangle GetClip()
        {
            if (!Win32.GetClipCursor(out var rect)) {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return rect;
        }

        public static void SetClip(in Rectangle value)
        {
            var rect = (Win32.RECT) value;
            if (!Win32.ClipCursor(ref rect)) {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }

        public static void ClearClip()
        {
            if (!Win32.ClipCursor(IntPtr.Zero)) {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }


        public static void SwitchToInputDesktop()
        {
            try {
                // var threadCurrent = GetCurrentDesktop();
                var inputCurrent = GetInputDesktop();

                if (inputCurrent != IntPtr.Zero) {
                    try {
                        // if (threadCurrent != inputCurrent) {
                        SetCurrentDesktop(inputCurrent);
                        // }
                    }
                    finally {
                        Win32.CloseDesktop(inputCurrent);
                    }
                }
            }
            catch (Win32Exception e) {
                const int accessIsDenied = 5;

                if (e.NativeErrorCode != accessIsDenied) {
                    throw;
                }
            }
        }

        private static IntPtr GetCurrentDesktop()
        {
            var hDesktop = Win32.GetThreadDesktop(Win32.GetCurrentThreadId());
            if (hDesktop == IntPtr.Zero) {
                var error = Marshal.GetLastWin32Error();
                if (error != 0) {
                    throw new Win32Exception(error);
                }
            }

            return hDesktop;
        }

        private static void SetCurrentDesktop(IntPtr desktopHandle)
        {
            var success = Win32.SetThreadDesktop(desktopHandle);
            if (!success) {
                var error = Marshal.GetLastWin32Error();
                if (error == 170 /*The requested resource is in use*/) {
                    return;
                }

                throw new Win32Exception(error);
            }
        }

        private static IntPtr GetInputDesktop()
        {
            var hDesktop = Win32.OpenInputDesktop(dwFlags: 0, fInherit: false, dwDesiredAccess: 0);
            if (hDesktop == IntPtr.Zero) {
                var error = Marshal.GetLastWin32Error();
                if (error != 0) {
                    throw new Win32Exception(error);
                }
            }

            return hDesktop;
        }
    }
}
