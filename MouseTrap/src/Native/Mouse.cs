using System.ComponentModel;
using System.Runtime.InteropServices;


namespace MouseTrap.Native; 

internal class Mouse {
    public static bool MoveCursor(int x, int y)
    {
        return Win32.SetCursorPos(x, y) != Win32.BOOL.False;
    }

    public static bool TryGetPosition(out Point point)
    {
        return Win32.GetCursorPos(out point) != Win32.BOOL.False;
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


    public static bool IsInputDesktop()
    {
        var inputCurrent = GetCurrentDesktop();
        return IsInputDesktop(inputCurrent);
    }

    private static bool IsInputDesktop(IntPtr hDesktop)
    {
        const int UOI_IO = 6;
        var isInputDesktop = Win32.BOOL.False;
        var success = Win32.GetUserObjectInformation(hDesktop, UOI_IO, ref isInputDesktop, sizeof(Win32.BOOL), out _);
        if (!success) {
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error);
        }

        return isInputDesktop == Win32.BOOL.True;
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


    public static void SwitchToInputDesktop()
    {
        try {
            var inputCurrent = GetInputDesktop();
            if (inputCurrent != IntPtr.Zero) {
                try {
                    SetCurrentDesktop(inputCurrent);
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


    private static void SetCurrentDesktop(IntPtr desktopHandle)
    {
        // NOTE: It is impossible to set the thread desktop
        // after using SetWindowsHookEx() in a Thread!
        var success = Win32.SetThreadDesktop(desktopHandle);
        if (!success) {
            var error = Marshal.GetLastWin32Error();
            if (error == 170 /*The requested resource is in use*/) {
                throw new Exception("The requested resource is in use. Are there any hooks 'SetWindowsHookEx()' registered for this Thread?", new Win32Exception(error));
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